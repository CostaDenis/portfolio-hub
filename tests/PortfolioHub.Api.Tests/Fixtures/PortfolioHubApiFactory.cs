using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioHub.Api.Options;
using PortfolioHub.Api.Tests.Providers;
using PortfolioHub.Application.Providers;
using PortfolioHub.Infrastructure.Data;

namespace PortfolioHub.Api.Tests.Fixtures;

public sealed class PortfolioHubApiFactory(string connectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        //rate limiting para testes
        builder.UseSetting("PriceSync:ManualSyncPermitLimit", "1000");

        builder.ConfigureTestServices(services =>
        {
            //Aplica a configuração do Bd de teste
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<IMarketPriceProvider>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddSingleton<IMarketPriceProvider>(new FakeMarketPriceProvider());

            services.Configure<PriceSyncOptions>(options => options.Enabled = false);
        });
    }
}
