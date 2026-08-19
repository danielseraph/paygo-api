using PayGo.Core.Enums;

namespace PayGo.Model.Resources;

public class TransactionResourceParameters : PaginationParameters
{
    public Guid? MerchantId { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentProvider? Provider { get; set; }
    public RiskLevel? RiskLevel { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
