using PayGo.Integrations.Paystack.DTOs;

namespace PayGo.Integrations.Paystack.Abstractions;
public interface IPaystackService
{
    /// <summary>
    /// Initializes a new payment transaction on Paystack.
    /// Returns an authorization URL the customer is redirected to.
    /// </summary>
    Task<PaystackInitializeResponse> InitializeTransactionAsync(PaystackInitializeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a transaction by its reference. This is the source of truth.
    /// NEVER trust a client-side status — always verify server-side.
    /// </summary>
    Task<PaystackVerifyResponse> VerifyTransactionAsync(string reference,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Validate the webhook request from Paystack to ensure it is legitimate.
    /// Returns true if the request is valid, false otherwise.
    /// </summary>
    Task<bool> ValidateWebhookRequestAsync(string payload, string paystackSignatureHeader);
}
