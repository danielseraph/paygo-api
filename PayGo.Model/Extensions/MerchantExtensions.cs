using PayGo.Model.DTOs;
using PayGo.Model.Entities;

namespace PayGo.Model.Extensions;

public static class MerchantExtensions
{
    public static MerchantDto ToDto(this Merchant entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new MerchantDto
        {
            Id = entity.Id,
            BusinessName = entity.BusinessName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            BusinessAddress = entity.BusinessAddress,
            LogoUrl = entity.LogoUrl,
            WebsiteUrl = entity.WebsiteUrl,
            Status = entity.Status,
            CurrencyCode = entity.CurrencyCode,
            ApiKey = entity.ApiKey,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
