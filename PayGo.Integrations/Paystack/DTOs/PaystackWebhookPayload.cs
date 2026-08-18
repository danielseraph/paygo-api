using System.Text.Json.Serialization;

namespace PayGo.Integrations.Paystack.DTOs;

/// <summary>
/// Represents the top-level shape of a Paystack webhook event.
/// The inner Data object mirrors PaystackVerifyData since both
/// represent a transaction state.
/// </summary>
public class PaystackWebhookPayload
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty; // e.g. "charge.success"

    [JsonPropertyName("data")]
    public PaystackVerifyData? Data { get; set; }
}