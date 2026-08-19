using PayGo.Core.Enums;

namespace PayGo.Model.Responses;

public class LedgerEntryResponse
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; }
}
