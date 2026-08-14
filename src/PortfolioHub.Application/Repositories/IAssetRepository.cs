using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Repositories;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken);
    Task<Asset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken);
}