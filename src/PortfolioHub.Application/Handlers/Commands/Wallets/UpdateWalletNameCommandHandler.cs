using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class UpdateWalletNameCommandHandler(IWalletRepository walletRepository, WalletFinder walletFinder)
{

    public async Task HandleAsync(UpdateWalletNameCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        wallet.UpdateName(command.WalletName);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }
}