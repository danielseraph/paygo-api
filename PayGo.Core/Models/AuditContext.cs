namespace PayGo.Core.Models;

/// <summary>
/// Captures who performed an action and when.
/// Passed into service methods that create AuditLog entries.
/// Decouples audit tracking from IHttpContextAccessor in the service layer.
/// </summary>
public class AuditContext
{
    public string Actor { get; set; } = "system";        // merchantId, userId, or "system"
    public string ActorType { get; set; } = "system";    // "merchant", "admin", "system"
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a system-generated audit context (background jobs, webhooks, etc.)
    /// </summary>
    public static AuditContext System() => new()
    {
        Actor = "system",
        ActorType = "system",
        Timestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Creates an audit context for an authenticated merchant action.
    /// </summary>
    public static AuditContext ForMerchant(string merchantId, string? ipAddress = null)
        => new()
        {
            Actor = merchantId,
            ActorType = "merchant",
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };
}
