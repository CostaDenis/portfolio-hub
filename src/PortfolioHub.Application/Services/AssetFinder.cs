using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Services;

public class AssetFinder(IAssetRepository assetRepository)
{

    public async Task<Asset> GetRequiredAsync(Guid assetId, CancellationToken cancellationToken)
        => await assetRepository.GetByIdAsync(assetId, cancellationToken)
            ?? throw new AssetNotFoundException();
}