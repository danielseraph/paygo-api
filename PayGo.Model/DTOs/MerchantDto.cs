using PayGo.Core.Enums;

namespace PayGo.Model.DTOs;

public class MerchantDto
{
    public Guid Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? BusinessAddress { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public MerchantStatus Status { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public string? ApiKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
