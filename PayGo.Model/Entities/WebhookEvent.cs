using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;

public class WebhookEvent : BaseEntity
{
    public string Reference { get; set; } = string.Empty;
    public PaymentProvider Provider { get; set; }
    public string EventType { get; set; } = string.Empty;      // e.g. "charge.success"
    public string PayloadJson { get; set; } = string.Empty;
    public WebhookStatus Status { get; set; } = WebhookStatus.Received;
    public string? ProcessingError { get; set; }
    public int RetryCount { get; set; } = 0;
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Provider's unique event ID — used for idempotency checks.
    /// </summary>
    public string? ProviderEventId { get; set; }
}