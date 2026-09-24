using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Repositories;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken);
    Task<Asset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken);
    Task CreateAsync(Asset asset, CancellationToken cancellationToken);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken);
    Task<bool> ExistsByTickerAsync(Ticker ticker, CancellationToken cancellationToken);
}