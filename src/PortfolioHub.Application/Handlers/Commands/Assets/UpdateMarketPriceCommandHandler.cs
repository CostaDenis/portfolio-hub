using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Assets;

public class UpdateMarketPriceCommandHandler(IAssetRepository assetRepository, AssetFinder assetFinder)
{

    public async Task HandleAsync(UpdateMarketPriceCommand command, CancellationToken cancellationToken)
    {
        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        asset.UpdateMarketPrice(command.MarketPrice);

        await assetRepository.UpdateAsync(asset, cancellationToken);
    }
}