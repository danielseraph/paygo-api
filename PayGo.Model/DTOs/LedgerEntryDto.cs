using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class LedgerEntryDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; }
}
