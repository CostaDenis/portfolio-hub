using Microsoft.EntityFrameworkCore;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Infrastructure.Data;

namespace PortfolioHub.Infrastructure.Repositories;

public class WalletRepository(AppDbContext context) : IWalletRepository
{
    public async Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
        => await context.Wallets
            .Include(x => x.Transactions)
                .ThenInclude(x => x.Asset)
            .Include(x => x.Dividends)
                .ThenInclude(x => x.Asset)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == walletId, cancellationToken);

    public async Task CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        await context.Wallets.AddAsync(wallet, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        context.Wallets.Update(wallet);
        await context.SaveChangesAsync(cancellationToken);
    }
}