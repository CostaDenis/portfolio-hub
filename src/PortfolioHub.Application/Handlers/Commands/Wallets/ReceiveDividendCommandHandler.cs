using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class ReceiveDividendCommandHandler(IWalletRepository walletRepository,
    WalletFinder walletFinder, AssetFinder assetFinder)
{

    public async Task HandleAsync(ReceiveDividendCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        wallet.ReceiveDividend(asset, command.ValuePerShare, command.Date);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }
}