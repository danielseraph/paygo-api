using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
