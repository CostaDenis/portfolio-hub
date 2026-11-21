using Microsoft.EntityFrameworkCore;
using PortfolioHub.Infrastructure.Data;

namespace PortfolioHub.Api.Extensions;

public static class AppExtension
{

    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapSwagger();
    }

    public static void ApplyMigrations(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
            return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}