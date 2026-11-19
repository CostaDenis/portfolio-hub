using Microsoft.Extensions.Logging;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Providers;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Handlers.Commands.Assets;

public class SyncMarketPricesCommandHandler(IAssetRepository assetRepository,
    IEnumerable<IMarketPriceProvider> providers,
    ILogger<SyncMarketPricesCommandHandler> logger)
{

    public async Task<SyncMarketPricesResultDTO> HandleAsync(SyncMarketPricesCommand command, CancellationToken cancellationToken)
    {
        var assets = await assetRepository.GetAllAssets(cancellationToken);

        var notFound = 0;
        var skipped = 0;
        List<Asset> updatedAssets = [];

        foreach (var assetGroup in assets.GroupBy(a => a.Type))
        {
            var provider = providers.FirstOrDefault(p => p.Supports(assetGroup.Key));

            if (provider is null)
            {
                notFound += assetGroup.Count();
                continue;
            }

            if (!provider.IsAvailableNow())
            {
                skipped += assetGroup.Count();
                continue;
            }

            IReadOnlyDictionary<string, decimal> prices;
            try
            {
                var tickers = assetGroup.Select(a => a.Ticker.Value).ToList();
                prices = await provider.GetPricesAsync(tickers, cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning(exception, "Falha ao buscar preços de {AssetType}", assetGroup.Key);
                notFound += assetGroup.Count();
                continue;
            }

            foreach (var asset in assetGroup)
            {
                if (!prices.TryGetValue(asset.Ticker.Value, out var price)
                    || price <= 0)
                {
                    notFound++;
                    continue;
                }

                asset.UpdateMarketPrice(new MarketPrice(price));
                updatedAssets.Add(asset);
            }
        }

        //evita de dar hit no banco caso
        //o(s) provedor(es) estejam fora do ar
        if (updatedAssets.Count > 0)
            await assetRepository.UpdateRangeAsync(updatedAssets, cancellationToken);

        return new SyncMarketPricesResultDTO(updatedAssets.Count, notFound, skipped);
    }
}
