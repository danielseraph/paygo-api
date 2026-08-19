namespace PayGo.Model.Requests;

public class InitializePaymentRequest
{
    public Guid MerchantId { get; set; }
    public decimal Amount { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Currency { get; set; } = "NGN";
    public string? CallbackUrl { get; set; }
    public string? Reference { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
