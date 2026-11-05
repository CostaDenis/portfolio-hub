using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletPositionQueryHandler(WalletFinder walletFinder)
{
    public async Task<IReadOnlyCollection<WalletPositionDTO>> HandleAsync(
        GetWalletPositionQuery query,
        CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(query.WalletId, cancellationToken);
        List<WalletPositionDTO> positions = [];

        foreach (var asset in wallet.GetHeldAssets())
        {
            positions.Add(new WalletPositionDTO(
                asset.Id,
                asset.Ticker,
                asset.Name,
                wallet.GetCurrentQuantity(asset),
                asset.MarketPrice.Price.Value));
        }

        return positions.AsReadOnly();
    }
}
