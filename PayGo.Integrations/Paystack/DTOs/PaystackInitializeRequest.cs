using System.Text.Json.Serialization;

namespace PayGo.Integrations.Paystack.DTOs;

public class PaystackInitializeRequest
{
    /// <summary>
    /// The email address of the customer making the payment. 
    /// This is required for sending payment notifications and receipts.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Amount in the smallest currency unit 
    /// (e.g., kobo for NGN, cents for USD). For example, 
    /// to charge 100 NGN, you would set this value to 10000.
    /// </summary>]
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "NGN";

    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}
