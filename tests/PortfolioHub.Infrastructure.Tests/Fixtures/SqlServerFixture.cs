using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Infrastructure.Data;
using Testcontainers.MsSql;

namespace PortfolioHub.Infrastructure.Tests.Fixtures;

public sealed class SqlServerFixture : IAsyncDisposable
{

    private readonly MsSqlContainer _container =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    public string ConnectionString { get; private set; } = null!;

    public async Task StartAsync()
    {
        await _container.StartAsync();

        var connectionStringBuilder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "PortfolioHubTests"
        };

        ConnectionString = connectionStringBuilder.ConnectionString;
    }

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateDbContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public ValueTask DisposeAsync()
        => _container.DisposeAsync();
}
