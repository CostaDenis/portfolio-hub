using Microsoft.EntityFrameworkCore;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Data;

namespace PortfolioHub.Infrastructure.Repositories;

public class AssetRepository(AppDbContext context) : IAssetRepository
{
    public async Task<Asset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
        => await context.Assets.FirstOrDefaultAsync(x => x.Id == assetId, cancellationToken);

    public async Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken)
        => await context.Assets.AsNoTracking().ToListAsync(cancellationToken);

    public async Task CreateAsync(Asset asset, CancellationToken cancellationToken)
    {
        await context.Assets.AddAsync(asset, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Asset asset, CancellationToken cancellationToken)
    {
        context.Assets.Update(asset);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTickerAsync(Ticker ticker, CancellationToken cancellationToken)
    {
        string normalizedTicker = ticker;

        return await context.Assets
            .AnyAsync(x => x.Ticker == normalizedTicker, cancellationToken);
    }
}