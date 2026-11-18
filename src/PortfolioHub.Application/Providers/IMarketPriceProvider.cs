using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.Providers;

public interface IMarketPriceProvider
{
    EAssetType AssetType { get; }

    //ticker->preco
    Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync
        (IReadOnlyCollection<string> tickers, CancellationToken cancellationToken);
}
