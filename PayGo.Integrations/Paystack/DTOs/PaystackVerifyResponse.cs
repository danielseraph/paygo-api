using System.Text.Json.Serialization;

namespace PayGo.Integrations.Paystack.DTOs;
public class PaystackVerifyResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public PaystackVerifyData? Data { get; set; }
}
public class PaystackVerifyData
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // "success", "failed", "abandoned"
    /// <summary>
    /// Amount in kobo. Divide by 100 to get NGN.
    /// </summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
    [JsonPropertyName("channel")]
    public string Channel { get; set; } = string.Empty; // "card", "bank", "ussd"
    [JsonPropertyName("gateway_response")]
    public string GatewayResponse { get; set; } = string.Empty;
    [JsonPropertyName("paid_at")]
    public DateTime? PaidAt { get; set; }
    [JsonPropertyName("fees")]
    public long? Fees { get; set; }
    [JsonPropertyName("customer")]
    public PaystackCustomer? Customer { get; set; }
    [JsonPropertyName("authorization")]
    public PaystackAuthorization? Authorization { get; set; }
}
public class PaystackCustomer
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}
public class PaystackAuthorization
{
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }
    [JsonPropertyName("last4")]
    public string? Last4 { get; set; }
    [JsonPropertyName("bank")]
    public string? Bank { get; set; }
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }
}