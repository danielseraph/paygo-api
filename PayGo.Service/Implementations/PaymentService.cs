using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Integrations.Paystack.Abstractions;
using PayGo.Integrations.Paystack.DTOs;
using PayGo.Model.Entities;
using PayGo.Model.Requests;
using PayGo.Model.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;
using System.Text.Json;

namespace PayGo.Service.Implementations;

public class PaymentService : IPaymentService
{
    private readonly PayGoDbContext _dbContext;
    private readonly IPaystackService _paystackService;
    private readonly ILedgerService _ledgerService;
    private readonly IFraudRiskService _fraudRiskService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        PayGoDbContext dbContext,
        IPaystackService paystackService,
        ILedgerService ledgerService,
        IFraudRiskService fraudRiskService,
        ILogger<PaymentService> logger)
    {
        _dbContext = dbContext;
        _paystackService = paystackService;
        _ledgerService = ledgerService;
        _fraudRiskService = fraudRiskService;
        _logger = logger;
    }

    public async Task<ApiResponse<InitializePaymentResponse>> InitializePaymentAsync(InitializePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var merchant = await _dbContext.Merchants.FirstOrDefaultAsync(m => m.Id == request.MerchantId, cancellationToken);
        if (merchant == null)
        {
            return new ApiResponse<InitializePaymentResponse>().FailureResponse("Merchant not found", 404);
        }

        // Fraud evaluation
        var riskRes = await _fraudRiskService.EvaluateTransactionRiskAsync(request.MerchantId, request.Amount, request.Email, cancellationToken);
        var riskLevel = riskRes.Success ? riskRes.Data : RiskLevel.Low;

        string reference = string.IsNullOrWhiteSpace(request.Reference) 
            ? $"PAYGO-{Guid.NewGuid().ToString("N")[..12].ToUpper()}" 
            : request.Reference;

        long amountInKobo = (long)(request.Amount * 100);

        var paystackRequest = new PaystackInitializeRequest
        {
            Email = request.Email,
            Amount = amountInKobo,
            Currency = request.Currency,
            CallbackUrl = request.CallbackUrl,
            Reference = reference
        };

        var paystackResponse = await _paystackService.InitializeTransactionAsync(paystackRequest, cancellationToken);

        if (!paystackResponse.Status || paystackResponse.Data == null)
        {
            return new ApiResponse<InitializePaymentResponse>().FailureResponse(
                string.IsNullOrWhiteSpace(paystackResponse.Message) ? "Payment initialization failed on gateway" : paystackResponse.Message, 400);
        }

        var transaction = new Transaction
        {
            MerchantId = request.MerchantId,
            Reference = reference,
            Provider = PaymentProvider.Paystack,
            Amount = request.Amount,
            NetAmount = request.Amount,
            CurrencyCode = request.Currency,
            Status = PaymentStatus.Pending,
            Type = TransactionType.Payment,
            CustomerEmail = request.Email,
            RiskLevel = riskLevel,
            IsVerified = false
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new InitializePaymentResponse
        {
            AuthorizationUrl = paystackResponse.Data.AuthorizationUrl,
            AccessCode = paystackResponse.Data.AccessCode,
            Reference = reference,
            Amount = request.Amount
        };

        return new ApiResponse<InitializePaymentResponse>().SuccessResponse(response, "Payment initialized successfully", 201);
    }

    public async Task<ApiResponse<PaymentVerificationResponse>> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Reference == reference, cancellationToken);

        if (transaction == null)
        {
            return new ApiResponse<PaymentVerificationResponse>().FailureResponse("Transaction reference not found", 404);
        }

        if (transaction.IsVerified && transaction.Status == PaymentStatus.Success)
        {
            return new ApiResponse<PaymentVerificationResponse>().SuccessResponse(MapToVerificationResponse(transaction), "Transaction already verified");
        }

        var paystackResponse = await _paystackService.VerifyTransactionAsync(reference, cancellationToken);

        if (!paystackResponse.Status || paystackResponse.Data == null)
        {
            return new ApiResponse<PaymentVerificationResponse>().FailureResponse("Payment verification failed on provider gateway", 400);
        }

        var data = paystackResponse.Data;
        transaction.ProviderStatus = data.Status;
        transaction.ProviderResponseJson = data.GatewayResponse;
        transaction.Channel = data.Channel;
        transaction.VerifiedAt = DateTime.UtcNow;

        if (data.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
            transaction.Status = PaymentStatus.Success;
            transaction.IsVerified = true;
            transaction.ProviderFee = data.Fees.HasValue ? data.Fees.Value / 100m : 0m;
            transaction.NetAmount = transaction.Amount - (transaction.ProviderFee ?? 0m);

            // Record double-entry ledger credit for merchant
            await _ledgerService.RecordEntryAsync(new RecordLedgerEntryRequest
            {
                MerchantId = transaction.MerchantId,
                TransactionId = transaction.Id,
                Amount = transaction.NetAmount,
                EntryType = LedgerEntryType.Credit,
                Description = $"Payment received via Paystack reference: {reference}",
                Reference = reference
            }, cancellationToken);
        }
        else
        {
            transaction.Status = PaymentStatus.Failed;
            transaction.IsVerified = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse<PaymentVerificationResponse>().SuccessResponse(MapToVerificationResponse(transaction), "Transaction verification updated successfully");
    }

    //public async Task<ApiResponse<bool>> ProcessWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default)
    //{
    //    bool isValid = await _paystackService.ValidateWebhookRequestAsync(payload, signatureHeader);
    //    if (!isValid)
    //    {
    //        return new ApiResponse<bool>().FailureResponse("Invalid webhook signature", 401);
    //    }

    //    var webhookEvent = new WebhookEvent
    //    {
    //        Reference = $"WH-{Guid.NewGuid().ToString("N")[..8]}",
    //        Provider = PaymentProvider.Paystack,
    //        EventType = "charge.success",
    //        PayloadJson = payload,
    //        Status = WebhookStatus.Processed,
    //        ProcessedAt = DateTime.UtcNow
    //    };

    //    _dbContext.WebhookEvents.Add(webhookEvent);
    //    await _dbContext.SaveChangesAsync(cancellationToken);

    //    return new ApiResponse<bool>().SuccessResponse(true, "Webhook processed idempotently");
    //}

    public async Task<ApiResponse<bool>> ProcessWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default)
    {
        // Step 1: Validate the HMAC-SHA512 signature from Paystack
        bool isValid = await _paystackService.ValidateWebhookRequestAsync(payload, signatureHeader);
        if (!isValid)
        {
            return new ApiResponse<bool>().FailureResponse("Invalid webhook signature", 401);
        }

        // Step 2: Parse the raw JSON payload into a typed object
        PaystackWebhookPayload? webhookPayload;
        try
        {
            webhookPayload = JsonSerializer.Deserialize<PaystackWebhookPayload>(
                payload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Paystack webhook payload.");
            return new ApiResponse<bool>().FailureResponse("Malformed webhook payload", 400);
        }

        if (webhookPayload?.Data == null)
        {
            return new ApiResponse<bool>().FailureResponse("Webhook payload is missing data", 400);
        }

        string eventType = webhookPayload.Event;          // e.g. "charge.success"
        string reference = webhookPayload.Data.Reference; // Paystack transaction reference

        // Step 3: Idempotency check — have we already processed this exact event?
        // We use the reference + eventType combination as the unique key.
        bool alreadyProcessed = await _dbContext.WebhookEvents
            .AnyAsync(w => w.Reference == reference && w.EventType == eventType, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogInformation("Duplicate webhook received for reference {Reference}, event {Event}. Skipping.", reference, eventType);
            return new ApiResponse<bool>().SuccessResponse(true, "Webhook already processed");
        }

        // Step 4: Store the raw event FIRST, with status Received.
        // We always persist before acting so we have a full audit trail even if processing fails.
        var webhookEvent = new WebhookEvent
        {
            Reference = reference,
            Provider = PaymentProvider.Paystack,
            EventType = eventType,  // ← now correctly from the parsed payload, not hardcoded
            PayloadJson = payload,
            Status = WebhookStatus.Received,
        };

        _dbContext.WebhookEvents.Add(webhookEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Step 5: Route on event type — only act on what we know
        if (eventType == "charge.success")
        {
            await HandleChargeSuccessAsync(webhookPayload.Data, webhookEvent, cancellationToken);
        }
        else
        {
            // We received a valid signed event we don't handle yet — log it, don't fail
            _logger.LogInformation("Unhandled Paystack webhook event type: {EventType}", eventType);
            webhookEvent.Status = WebhookStatus.Processed;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new ApiResponse<bool>().SuccessResponse(true, "Webhook received and processed");
    }
    private async Task HandleChargeSuccessAsync(PaystackVerifyData data, WebhookEvent webhookEvent, CancellationToken cancellationToken)
    {
        // Find the internal transaction by the Paystack reference
        var transaction = await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Reference == data.Reference, cancellationToken);

        if (transaction == null)
        {
            // Paystack sent us a success for a reference we don't recognise — log and mark
            _logger.LogWarning("Webhook charge.success received for unknown reference: {Reference}", data.Reference);
            webhookEvent.Status = WebhookStatus.Failed;
            webhookEvent.ProcessingError = $"No internal transaction found for reference: {data.Reference}";
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        // Guard: don't double-process a transaction that's already verified as successful
        if (transaction.IsVerified && transaction.Status == PaymentStatus.Success)
        {
            _logger.LogInformation("Webhook charge.success for already-verified reference {Reference}. Skipping.", data.Reference);
            webhookEvent.Status = WebhookStatus.Processed;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        // Update transaction fields from the webhook data
        transaction.Status = PaymentStatus.Success;
        transaction.IsVerified = true;
        transaction.ProviderStatus = data.Status;
        transaction.ProviderResponseJson = data.GatewayResponse;
        transaction.Channel = data.Channel;
        transaction.VerifiedAt = data.PaidAt ?? DateTime.UtcNow;
        transaction.ProviderFee = data.Fees.HasValue ? data.Fees.Value / 100m : 0m;
        transaction.NetAmount = transaction.Amount - (transaction.ProviderFee ?? 0m);

        // Enrich with customer info from the webhook if we don't already have it
        if (data.Customer != null)
        {
            transaction.CustomerEmail = data.Customer.Email;
            transaction.CustomerName = $"{data.Customer.FirstName} {data.Customer.LastName}".Trim();
            transaction.CustomerPhone = data.Customer.Phone;
        }

        // Record the double-entry ledger credit for the merchant
        await _ledgerService.RecordEntryAsync(new RecordLedgerEntryRequest
        {
            MerchantId = transaction.MerchantId,
            TransactionId = transaction.Id,
            Amount = transaction.NetAmount,
            EntryType = LedgerEntryType.Credit,
            CurrencyCode = transaction.CurrencyCode,
            Description = $"Payment confirmed via Paystack webhook. Reference: {data.Reference}",
            Reference = data.Reference
        }, cancellationToken);

        // Mark the webhook event as fully processed
        webhookEvent.Status = WebhookStatus.Processed;
        webhookEvent.ProcessedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Webhook charge.success processed. Reference: {Reference}, Amount: {Amount}, MerchantId: {MerchantId}",
            data.Reference, transaction.NetAmount, transaction.MerchantId);
    }
    private static PaymentVerificationResponse MapToVerificationResponse(Transaction transaction)
    {
        return new PaymentVerificationResponse
        {
            Reference = transaction.Reference,
            Status = transaction.Status,
            Amount = transaction.Amount,
            Currency = transaction.CurrencyCode,
            GatewayResponse = transaction.ProviderResponseJson ?? string.Empty,
            PaidAt = transaction.VerifiedAt,
            Channel = transaction.Channel ?? string.Empty,
            MerchantId = transaction.MerchantId
        };
    }
}
