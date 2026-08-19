using PayGo.Core.Enums;

namespace PayGo.Model.Resources;

public class MerchantResourceParameters : PaginationParameters
{
    public MerchantStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
}
