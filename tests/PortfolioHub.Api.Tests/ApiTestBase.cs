using PortfolioHub.Api.Tests.Fixtures;
using PortfolioHub.Infrastructure.Data;

namespace PortfolioHub.Api.Tests;

public abstract class ApiTestBase
{
    protected HttpClient Client { get; private set; } = null!;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        await TestAssemblyHooks.Database.ResetDatabaseAsync();
        Client = TestAssemblyHooks.Factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup() => Client?.Dispose();

    protected AppDbContext CreateDbContext()
        => TestAssemblyHooks.Database.CreateDbContext();
}
