using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletAssetPositionQueryHandler(
    WalletFinder walletFinder,
    AssetFinder assetFinder)
{
    public async Task<WalletPositionDTO> HandleAsync(GetWalletAssetPositionQuery query, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(query.WalletId, cancellationToken);
        var asset = await assetFinder.GetRequiredAsync(query.AssetId, cancellationToken);

        return new WalletPositionDTO(
            asset.Id,
            asset.Ticker,
            asset.Name,
            wallet.GetCurrentQuantity(asset),
            asset.MarketPrice.Price.Value);
    }
}
