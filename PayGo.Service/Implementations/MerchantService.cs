using Microsoft.EntityFrameworkCore;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Model.Entities;
using PayGo.Model.Requests;
using PayGo.Model.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;

namespace PayGo.Service.Implementations;

public class MerchantService : IMerchantService
{
    private readonly PayGoDbContext _dbContext;

    public MerchantService(PayGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<MerchantResponse>> CreateMerchantAsync(CreateMerchantRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken);

        if (existing != null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse($"Merchant with email {request.Email} already exists.");
        }

        var merchant = new Merchant
        {
            BusinessName = request.BusinessName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            BusinessAddress = request.BusinessAddress,
            CurrencyCode = request.CurrencyCode,
            Status = MerchantStatus.Active,
            ApiKey = $"pk_live_{Guid.NewGuid().ToString("N")}",
            ApiSecret = $"sk_live_{Guid.NewGuid().ToString("N")}"
        };

        _dbContext.Merchants.Add(merchant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(merchant);
        return new ApiResponse<MerchantResponse>().SuccessResponse(response, "Merchant created successfully", 201);
    }

    public async Task<ApiResponse<MerchantResponse>> GetMerchantByIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        var merchant = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Id == merchantId, cancellationToken);

        if (merchant == null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse("Merchant not found", 404);
        }

        return new ApiResponse<MerchantResponse>().SuccessResponse(MapToResponse(merchant));
    }

    public async Task<ApiResponse<MerchantResponse>> GetMerchantByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var merchant = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);

        if (merchant == null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse("Merchant not found", 404);
        }

        return new ApiResponse<MerchantResponse>().SuccessResponse(MapToResponse(merchant));
    }

    private static MerchantResponse MapToResponse(Merchant merchant)
    {
        return new MerchantResponse
        {
            Id = merchant.Id,
            BusinessName = merchant.BusinessName,
            Email = merchant.Email,
            PhoneNumber = merchant.PhoneNumber,
            Status = merchant.Status,
            CurrencyCode = merchant.CurrencyCode,
            ApiKey = merchant.ApiKey,
            CreatedAt = merchant.CreatedAt
        };
    }
}
