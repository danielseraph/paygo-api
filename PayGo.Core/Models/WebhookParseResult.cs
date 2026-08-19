namespace PayGo.Core.Models;

/// <summary>
/// Represents the outcome of parsing an inbound webhook payload.
/// Decouples raw JSON parsing from the business logic in PaymentService.
/// </summary>
public class WebhookParseResult
{
    public bool IsValid { get; set; }
    public string EventType { get; set; } = string.Empty;    // e.g. "charge.success"
    public string? Reference { get; set; }                   // transaction reference
    public string? ProviderEventId { get; set; }             // idempotency key from provider
    public string? ParseError { get; set; }                  // set when IsValid = false
    public Dictionary<string, object> Data { get; set; } = new();

    public static WebhookParseResult Failure(string reason) => new()
    {
        IsValid = false,
        ParseError = reason
    };

    public static WebhookParseResult Success(string eventType, string? reference, string? providerEventId = null)
        => new()
        {
            IsValid = true,
            EventType = eventType,
            Reference = reference,
            ProviderEventId = providerEventId
        };
}
