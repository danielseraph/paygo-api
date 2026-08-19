using PayGo.Model.DTOs;
using PayGo.Model.Entities;

namespace PayGo.Model.Extensions;

public static class PaymentLinkExtensions
{
    public static PaymentLinkDto ToDto(this PaymentLink entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new PaymentLinkDto
        {
            Id = entity.Id,
            MerchantId = entity.MerchantId,
            Reference = entity.Reference,
            Title = entity.Title,
            Description = entity.Description,
            Amount = entity.Amount,
            CurrencyCode = entity.CurrencyCode,
            Status = entity.Status,
            RedirectUrl = entity.RedirectUrl,
            ExpiresAt = entity.ExpiresAt,
            IsOneTime = entity.IsOneTime,
            CreatedAt = entity.CreatedAt
        };
    }
}
