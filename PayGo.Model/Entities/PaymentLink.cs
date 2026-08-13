using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;
// Represents a payment link entity
public class PaymentLink : BaseEntity
{
    public Guid MerchantId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public PaymentLinkStatus Status { get; set; } = PaymentLinkStatus.Active;
    public string? RedirectUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsOneTime { get; set; } = true;
    public string? MetadataJson { get; set; }

    // Navigation
    public Merchant Merchant { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}