using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class ReverseDividendCommandHandler(IWalletRepository walletRepository,
    WalletFinder walletFinder)
{

    public async Task HandleAsync(ReverseDividendCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        wallet.ReverseDividend(command.DividendId);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

}
