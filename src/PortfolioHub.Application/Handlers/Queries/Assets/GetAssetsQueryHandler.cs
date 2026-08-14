using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Assets;
using PortfolioHub.Application.Repositories;

namespace PortfolioHub.Application.Handlers.Queries.Assets;

public class GetAssetsQueryHandler(IAssetRepository assetRepository)
{

    public async Task<List<AssetDTO>> HandleAsync(GetAssetsQuery query, CancellationToken cancellationToken)
    {
        var assets = await assetRepository.GetAllAssets(cancellationToken);

        List<AssetDTO> assetsDTOs = [];
        foreach (var asset in assets)
            assetsDTOs.Add(new(
                asset.Id,
                asset.Name,
                asset.Ticker,
                asset.Type,
                asset.MarketPrice.Price.Value
            ));

        return assetsDTOs;
    }
}