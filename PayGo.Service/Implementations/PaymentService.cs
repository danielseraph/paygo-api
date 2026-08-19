using Microsoft.EntityFrameworkCore;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Integrations.Paystack.Abstractions;
using PayGo.Integrations.Paystack.DTOs;
using PayGo.Model.Entities;
using PayGo.Model.Requests;
using PayGo.Model.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;

namespace PayGo.Service.Implementations;

public class PaymentService : IPaymentService
{
    private readonly PayGoDbContext _dbContext;
    private readonly IPaystackService _paystackService;
    private readonly ILedgerService _ledgerService;
    private readonly IFraudRiskService _fraudRiskService;

    public PaymentService(
        PayGoDbContext dbContext,
        IPaystackService paystackService,
        ILedgerService ledgerService,
        IFraudRiskService fraudRiskService)
    {
        _dbContext = dbContext;
        _paystackService = paystackService;
        _ledgerService = ledgerService;
        _fraudRiskService = fraudRiskService;
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

    public async Task<ApiResponse<bool>> ProcessWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default)
    {
        bool isValid = await _paystackService.ValidateWebhookRequestAsync(payload, signatureHeader);
        if (!isValid)
        {
            return new ApiResponse<bool>().FailureResponse("Invalid webhook signature", 401);
        }

        var webhookEvent = new WebhookEvent
        {
            Reference = $"WH-{Guid.NewGuid().ToString("N")[..8]}",
            Provider = PaymentProvider.Paystack,
            EventType = "charge.success",
            PayloadJson = payload,
            Status = WebhookStatus.Processed,
            ProcessedAt = DateTime.UtcNow
        };

        _dbContext.WebhookEvents.Add(webhookEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse<bool>().SuccessResponse(true, "Webhook processed idempotently");
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
