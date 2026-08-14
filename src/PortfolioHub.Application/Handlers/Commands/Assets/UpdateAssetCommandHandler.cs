using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Assets;

public class UpdateAssetCommandHandler(IAssetRepository assetRepository, AssetFinder assetFinder)
{
    public async Task HandleAsync(UpdateAssetCommand command, CancellationToken cancellationToken)
    {
        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        asset.UpdateName(command.AssetName);
        asset.UpdateTicker(command.Ticker);
        asset.UpdateType(command.Type);

        await assetRepository.UpdateAsync(asset, cancellationToken);
    }
}