using Microsoft.Extensions.DependencyInjection;
using PayGo.Service.Implementations;
using PayGo.Service.Interfaces;

namespace PayGo.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<IMerchantService, MerchantService>();
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<IFraudRiskService, FraudRiskService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
