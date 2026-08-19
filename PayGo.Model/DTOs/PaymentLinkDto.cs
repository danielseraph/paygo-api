using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class PaymentLinkDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public PaymentLinkStatus Status { get; set; }
    public string? RedirectUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsOneTime { get; set; }
    public DateTime CreatedAt { get; set; }
}
