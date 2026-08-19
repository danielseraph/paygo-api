using PayGo.Core.Responses;
using PayGo.Model.Requests;
using PayGo.Model.Responses;

namespace PayGo.Service.Interfaces;

public interface ILedgerService
{
    Task<ApiResponse<LedgerEntryResponse>> RecordEntryAsync(RecordLedgerEntryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<decimal>> GetMerchantBalanceAsync(Guid merchantId, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<LedgerEntryResponse>>> GetMerchantLedgerEntriesAsync(Guid merchantId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
