using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class WebhookEventDto
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public PaymentProvider Provider { get; set; }
    public string EventType { get; set; } = string.Empty;
    public WebhookStatus Status { get; set; }
    public string? ProcessingError { get; set; }
    public int RetryCount { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProviderEventId { get; set; }
    public DateTime CreatedAt { get; set; }
}
