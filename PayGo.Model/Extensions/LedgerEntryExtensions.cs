using PayGo.Model.DTOs;
using PayGo.Model.Entities;

namespace PayGo.Model.Extensions;

public static class LedgerEntryExtensions
{
    public static LedgerEntryDto ToDto(this LedgerEntry entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new LedgerEntryDto
        {
            Id = entity.Id,
            MerchantId = entity.MerchantId,
            TransactionId = entity.TransactionId,
            EntryType = entity.EntryType,
            TransactionType = entity.TransactionType,
            Amount = entity.Amount,
            BalanceAfter = entity.BalanceAfter,
            CurrencyCode = entity.CurrencyCode,
            Description = entity.Description,
            Reference = entity.Reference,
            CreatedAt = entity.CreatedAt
        };
    }
}
