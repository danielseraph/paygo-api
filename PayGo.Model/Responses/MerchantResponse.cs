using PayGo.Core.Enums;

namespace PayGo.Model.Responses;

public class MerchantResponse
{
    public Guid Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public MerchantStatus Status { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
    public string? ApiKey { get; set; }
    public DateTime CreatedAt { get; set; }
}
