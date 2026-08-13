using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;
// Represents a notification entity
public class Notification : BaseEntity
{
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public DateTime? SentAt { get; set; }

    // Navigation
    public Merchant Merchant { get; set; } = null!;
}