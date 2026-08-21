using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Handlers.Commands.Wallets;

public class CreateWalletCommandHandler(IWalletRepository walletRepository)
{

    public async Task<WalletDTO> HandleAsync(CreateWalletCommand command, CancellationToken cancellationToken)
    {
        Wallet wallet = new(command.Name);
        await walletRepository.CreateWalletAsync(wallet, cancellationToken);

        return new WalletDTO(wallet.Id, wallet.Name);
    }
}