using PayGo.Core.Enums;

namespace PayGo.Model.Resources;

public class LedgerResourceParameters : PaginationParameters
{
    public Guid? MerchantId { get; set; }
    public LedgerEntryType? EntryType { get; set; }
    public string? Reference { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
