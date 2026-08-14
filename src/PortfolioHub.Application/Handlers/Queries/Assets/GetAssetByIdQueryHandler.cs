using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Assets;
using PortfolioHub.Application.Repositories;

namespace PortfolioHub.Application.Handlers.Queries.Assets;

public class GetAssetByIdQueryHandler(IAssetRepository assetRepository)
{

    public async Task<AssetDTO?> HandleAsync(GetAssetByIdQuery query, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(query.AssetId, cancellationToken);

        if (asset is null)
            return null;

        return new AssetDTO(asset.Id, asset.Name, asset.Ticker, asset.Type, asset.MarketPrice.Price.Value);
    }
}