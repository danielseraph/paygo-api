using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;

namespace PayGo.Model.Entities;
// Represents a financial transaction
public class Transaction : BaseEntity
{
    public Guid MerchantId { get; set; }
    public string Reference { get; set; } = string.Empty;

    // Provider details
    public string? ProviderReference { get; set; }
    public PaymentProvider Provider { get; set; }
    public string? ProviderStatus { get; set; }
    public string? ProviderResponseJson { get; set; }

    // Financial details — decimal for money, never double
    public decimal Amount { get; set; }
    public decimal? ProviderFee { get; set; }
    public decimal NetAmount { get; set; }
    public string CurrencyCode { get; set; } = "NGN";

    // Status
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public TransactionType Type { get; set; } = TransactionType.Payment;

    // Customer info
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }

    // Payment link (nullable — not every payment comes from a link)
    public Guid? PaymentLinkId { get; set; }

    // Verification
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }

    // Metadata
    public string? Description { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }
    public string? Channel { get; set; } // e.g. "card", "bank", "ussd"

    // Risk
    public RiskLevel? RiskLevel { get; set; }
    public decimal? RiskScore { get; set; }

    // Navigation
    public Merchant Merchant { get; set; } = null!;
    public PaymentLink? PaymentLink { get; set; }
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
}