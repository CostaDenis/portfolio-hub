using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.Providers;

public interface IMarketPriceProvider
{
    bool Supports(EAssetType type);

    //ticker->preco
    Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync
        (IReadOnlyCollection<string> tickers, CancellationToken cancellationToken);
}
