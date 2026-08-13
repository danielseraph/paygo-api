using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;

public class LedgerEntry : BaseEntity
{
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public string? Description { get; set; }
    public string? Reference { get; set; }

    // Navigation
    public Merchant Merchant { get; set; } = null!;
    public Transaction? Transaction { get; set; }
}