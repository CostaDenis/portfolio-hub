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

        var assets = wallet.Transactions
            .Select(transaction => transaction.Asset)
            .DistinctBy(asset => asset.Id);

        foreach (var asset in assets)
        {
            var quantity = wallet.GetCurrentQuantity(asset);

            if (quantity == 0)
                continue;

            positions.Add(new WalletPositionDTO(
                asset.Id,
                asset.Ticker,
                asset.Name,
                quantity,
                asset.MarketPrice.Price.Value));
        }

        return positions.AsReadOnly();
    }
}
