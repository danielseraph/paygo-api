using PayGo.Model.DTOs;
using PayGo.Model.Entities;

namespace PayGo.Model.Extensions;

public static class TransactionExtensions
{
    public static TransactionDto ToDto(this Transaction entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new TransactionDto
        {
            Id = entity.Id,
            MerchantId = entity.MerchantId,
            Reference = entity.Reference,
            ProviderReference = entity.ProviderReference,
            Provider = entity.Provider,
            ProviderStatus = entity.ProviderStatus,
            Amount = entity.Amount,
            ProviderFee = entity.ProviderFee,
            NetAmount = entity.NetAmount,
            CurrencyCode = entity.CurrencyCode,
            Status = entity.Status,
            Type = entity.Type,
            CustomerEmail = entity.CustomerEmail,
            CustomerName = entity.CustomerName,
            CustomerPhone = entity.CustomerPhone,
            PaymentLinkId = entity.PaymentLinkId,
            IsVerified = entity.IsVerified,
            VerifiedAt = entity.VerifiedAt,
            Description = entity.Description,
            Channel = entity.Channel,
            RiskLevel = entity.RiskLevel,
            RiskScore = entity.RiskScore,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
