using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Commands.Assets;

public class UpdateAssetCommandHandler(IAssetRepository assetRepository, AssetFinder assetFinder)
{
    public async Task HandleAsync(UpdateAssetCommand command, CancellationToken cancellationToken)
    {
        var tickerAlreadyRegistred = await assetRepository
            .ExistsByTickerAsync(command.Ticker, cancellationToken);

        if (tickerAlreadyRegistred)
            throw new TickerAlreadyUsedException();

        var asset = await assetFinder.GetRequiredAsync(command.AssetId, cancellationToken);

        asset.UpdateName(command.AssetName);
        asset.UpdateTicker(command.Ticker);
        asset.UpdateType(command.Type);

        await assetRepository.UpdateAsync(asset, cancellationToken);
    }
}