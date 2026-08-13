using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;
// Represents an audit log entry for tracking changes in the system
public class AuditLog : BaseEntity
{
    public Guid? MerchantId { get; set; }
    public string? UserId { get; set; }
    public string Action { get; set; } = string.Empty;          // e.g. "Transaction.Verified"
    public string EntityType { get; set; } = string.Empty;      // e.g. "Transaction"
    public string? EntityId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}