using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;
// Represents a reconciliation record for a merchant's transaction
public class ReconciliationRecord : BaseEntity
{
    public Guid MerchantId { get; set; }
    public Guid? TransactionId { get; set; }
    public string? ProviderReference { get; set; }
    public ReconciliationStatus Status { get; set; } = ReconciliationStatus.Pending;

    // What was expected vs what was found
    public decimal? ExpectedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public string? ExpectedStatus { get; set; }
    public string? ActualStatus { get; set; }
    public string? MismatchReason { get; set; }
    public string? Notes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }

    // Navigation
    public Merchant Merchant { get; set; } = null!;
    public Transaction? Transaction { get; set; }
}