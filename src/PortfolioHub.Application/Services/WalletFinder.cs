using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Services;

public class WalletFinder(IWalletRepository walletRepository)
{

    public async Task<Wallet> GetRequiredAsync(Guid walletId, CancellationToken cancellationToken)
        => await walletRepository.GetByIdAsync(walletId, cancellationToken)
            ?? throw new WalletNotFoundException();
}