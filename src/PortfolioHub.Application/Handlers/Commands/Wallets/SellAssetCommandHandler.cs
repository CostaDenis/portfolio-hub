using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class SellAssetCommandHandler(IWalletRepository walletRepository,
    WalletFinder walletFinder, AssetFinder assetFinder)
{

    public async Task HandleAsync(SellAssetCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        wallet.SellAsset(asset, command.Quantity, command.UnitPrice);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }
}