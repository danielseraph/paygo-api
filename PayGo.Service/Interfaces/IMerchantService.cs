using PayGo.Core.Responses;
using PayGo.Model.Requests;
using PayGo.Model.Responses;

namespace PayGo.Service.Interfaces;

public interface IMerchantService
{
    Task<ApiResponse<MerchantResponse>> CreateMerchantAsync(CreateMerchantRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<MerchantResponse>> GetMerchantByIdAsync(Guid merchantId, CancellationToken cancellationToken = default);
    Task<ApiResponse<MerchantResponse>> GetMerchantByEmailAsync(string email, CancellationToken cancellationToken = default);
}
