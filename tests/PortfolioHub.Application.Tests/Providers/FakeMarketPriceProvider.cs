using PortfolioHub.Application.Providers;
using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.Tests.Providers;

public class FakeMarketPriceProvider(EAssetType assetType, bool shouldFail = false,
    IReadOnlyDictionary<string, decimal>? prices = null) : IMarketPriceProvider
{
    public EAssetType AssetType => assetType;

    public Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync(IReadOnlyCollection<string> tickers, CancellationToken cancellationToken)
    {
        if (shouldFail)
            throw new HttpRequestException("API inacessível");

        return Task.FromResult(prices ?? new Dictionary<string, decimal>());
    }
}
