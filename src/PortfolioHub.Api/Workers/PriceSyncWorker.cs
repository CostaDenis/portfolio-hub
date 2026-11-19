using Microsoft.Extensions.Options;
using PortfolioHub.Api.Options;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;

namespace PortfolioHub.Api.Workers;

public class PriceSyncWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<PriceSyncOptions> options,
    ILogger<PriceSyncWorker> logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.Enabled)
            return;

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.IntervalMinutes));

        do
        {
            await SyncAsync(cancellationToken);
        } while (await timer.WaitForNextTickAsync(cancellationToken));
    }

    private async Task SyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<SyncMarketPricesCommandHandler>();

            var result = await handler.HandleAsync(new SyncMarketPricesCommand(), cancellationToken);

            logger.LogInformation("Preços sincronizados: {Updated} atualizados, {NotFound} não encontrados",
                result.Updated, result.NotFound);

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            //api sendo desligada
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao sincronizar os preços. Nova tentativa na próxima rodada");
        }
    }
}
