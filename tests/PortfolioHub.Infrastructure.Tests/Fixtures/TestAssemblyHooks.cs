namespace PortfolioHub.Infrastructure.Tests.Fixtures;

[TestClass]
public static class TestAssemblyHooks
{

    public static SqlServerFixture Database { get; private set; } = null!;

    //antes de qualquer teste sobe o container do bd
    [AssemblyInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        Database = new SqlServerFixture();
        await Database.StartAsync();
    }

    //limpa o container quando todos os testes terminarem
    [AssemblyCleanup]
    public static async Task CleanupAsync()
        => await Database.DisposeAsync();
}
