using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Handlers.Commands.Assets;

public class CreateAssetCommandHandler(IAssetRepository assetRepository)
{
    public async Task<AssetDTO> HandleAsync(CreateAssetCommand command, CancellationToken cancellationToken)
    {
        var tickerAlreadyRegistred = await assetRepository
            .ExistsByTickerAsync(command.Ticker, cancellationToken);

        if (tickerAlreadyRegistred)
            throw new TickerAlreadyUsedException();

        Asset asset = new(command.AssetName, command.Ticker, command.Type, command.MarketPrice);
        await assetRepository.CreateAsync(asset, cancellationToken);

        return new AssetDTO(asset.Id, asset.Name, asset.Ticker,
            asset.Type, asset.MarketPrice.Price);
    }
}
