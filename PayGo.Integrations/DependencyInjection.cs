using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayGo.Core.Config;
using PayGo.Integrations.Paystack.Abstractions;
using PayGo.Integrations.Paystack.Services;

namespace PayGo.Integrations;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationsLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind Paystack settings from appsettings.json
        services.Configure<PaystackSettings>(
            configuration.GetSection(PaystackSettings.SectionName));
            
        // Register the typed HttpClient for Paystack
        // The Authorization header and BaseAddress are set here once globally.
        services.AddHttpClient<IPaystackService, PaystackService>((serviceProvider, client) =>
        {
            var settings = configuration
                .GetSection(PaystackSettings.SectionName)
                .Get<PaystackSettings>()!;
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.SecretKey}");
        });
        return services;
    }
}

