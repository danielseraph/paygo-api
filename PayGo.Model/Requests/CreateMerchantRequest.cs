namespace PayGo.Model.Requests;

public class CreateMerchantRequest
{
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? BusinessAddress { get; set; }
    public string CurrencyCode { get; set; } = "NGN";
}
