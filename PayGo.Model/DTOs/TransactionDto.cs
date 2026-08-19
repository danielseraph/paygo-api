using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string? ProviderReference { get; set; }
    public PaymentProvider Provider { get; set; }
    public string? ProviderStatus { get; set; }
    public decimal Amount { get; set; }
    public decimal? ProviderFee { get; set; }
    public decimal NetAmount { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public PaymentStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? PaymentLinkId { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Description { get; set; }
    public string? Channel { get; set; }
    public RiskLevel? RiskLevel { get; set; }
    public decimal? RiskScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
