using PortfolioHub.Infrastructure.Data;
using PortfolioHub.Infrastructure.Tests.Fixtures;

namespace PortfolioHub.Infrastructure.Tests;

public abstract class InfrastructureTestBase
{

    [TestInitialize]
    public async Task ResetDatabaseAsync()
        => await TestAssemblyHooks.Database.ResetDatabaseAsync();

    protected AppDbContext CreateDbContext()
        => TestAssemblyHooks.Database.CreateDbContext();
}
