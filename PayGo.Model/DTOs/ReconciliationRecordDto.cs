using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class ReconciliationRecordDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public string? ProviderReference { get; set; }
    public ReconciliationStatus Status { get; set; }
    public decimal? ExpectedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public string? ExpectedStatus { get; set; }
    public string? ActualStatus { get; set; }
    public string? MismatchReason { get; set; }
    public string? Notes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
