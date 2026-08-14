using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken);
    Task CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken);
    Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken);
}