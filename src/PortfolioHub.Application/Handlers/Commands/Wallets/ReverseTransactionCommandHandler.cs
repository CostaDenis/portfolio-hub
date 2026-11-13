using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class ReverseTransactionCommandHandler(IWalletRepository walletRepository,
    WalletFinder walletFinder)
{

    public async Task HandleAsync(ReverseTransactionCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(command.WalletId, cancellationToken);
        wallet.ReverseTransaction(command.TransactionId);

        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }
}
