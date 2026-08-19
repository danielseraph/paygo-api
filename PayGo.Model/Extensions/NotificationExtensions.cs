using PayGo.Model.DTOs;
using PayGo.Model.Entities;

namespace PayGo.Model.Extensions;

public static class NotificationExtensions
{
    public static NotificationDto ToDto(this Notification entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new NotificationDto
        {
            Id = entity.Id,
            MerchantId = entity.MerchantId,
            TransactionId = entity.TransactionId,
            Type = entity.Type,
            Channel = entity.Channel,
            Title = entity.Title,
            Message = entity.Message,
            IsRead = entity.IsRead,
            ReadAt = entity.ReadAt,
            SentAt = entity.SentAt,
            CreatedAt = entity.CreatedAt
        };
    }
}
