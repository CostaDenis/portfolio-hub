namespace PortfolioHub.Api.Tests.Fixtures;

[TestClass]
public static class TestAssemblyHooks
{
    public static SqlServerFixture Database { get; private set; } = null!;
    public static PortfolioHubApiFactory Factory { get; private set; } = null!;

    [AssemblyInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        Database = new SqlServerFixture();
        await Database.StartAsync();
        Factory = new PortfolioHubApiFactory(Database.ConnectionString);
    }

    [AssemblyCleanup]
    public static async Task CleanupAsync()
    {
        try
        {
            if (Factory is not null)
                await Factory.DisposeAsync();
        }
        finally
        {
            if (Database is not null)
                await Database.DisposeAsync();
        }
    }
}
