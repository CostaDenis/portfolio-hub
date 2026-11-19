using PortfolioHub.Application.Providers;
using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Api.Tests.Providers;

public class FakeMarketPriceProvider : IMarketPriceProvider
{
    public bool Supports(EAssetType type) => type == EAssetType.Cryptocurrency;
    public bool IsAvailableNow()
        => true;

    public const decimal BtcPrice = 367826m;

    public Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync(IReadOnlyCollection<string> tickers, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyDictionary<string, decimal>>(
                new Dictionary<string, decimal> { ["BTC"] = BtcPrice });

}
