using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioHub.Application.Providers;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Infrastructure.Data;
using PortfolioHub.Infrastructure.Providers;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Infrastructure;

public static class Infrastructure
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();

        services.AddHttpClient<IMarketPriceProvider, BinancePriceProvider>
            ("Binance", client => client.BaseAddress = new Uri("https://api.binance.com/"));

        services.AddHttpClient<IMarketPriceProvider, BrapiPriceProvider>("Brapi", client =>
        {
            client.BaseAddress = new Uri("https://brapi.dev/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["Brapi:Token"]);
        });

        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}