using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayGo.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services,
        IConfiguration cofig)
    {
        services.AddDbContext<PayGoDbContext>(options =>
        {
            options.UseNpgsql(cofig.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}