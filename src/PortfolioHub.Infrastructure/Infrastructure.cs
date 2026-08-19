using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Infrastructure.Data;
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

        return services;
    }
}