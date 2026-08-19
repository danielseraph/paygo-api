using PayGo.Core.Enums;

namespace PayGo.Model.Resources;

/// <summary>
/// Represents the currently authenticated merchant making the API request.
/// Populated from the API key/token on each request.
/// Injected via ICurrentMerchant interface in the service layer.
/// </summary>
public class CurrentMerchant
{
    public Guid Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "NGN";
    public MerchantStatus Status { get; set; }
    public bool IsActive => Status == MerchantStatus.Active;
}
