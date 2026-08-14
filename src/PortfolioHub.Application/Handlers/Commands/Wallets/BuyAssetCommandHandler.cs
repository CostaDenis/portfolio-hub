using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class BuyAssetCommandHandler(IWalletRepository walletRepository,
    WalletFinder walletFinder, AssetFinder assetFinder)
{

    public async Task HandleAsync(BuyAssetCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        wallet.BuyAsset(asset, command.Quantity, command.UnitPrice);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }
}