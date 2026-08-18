using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Assets;

[TestClass]
public class UpdateMarketPriceCommandHandlerTests
{
    [TestMethod]
    [TestCategory("UpdateMarketPriceCommandHandler tests")]
    public async Task Should_Update_MarketPrice()
    {
        var asset = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
                   EAssetType.Cryptocurrency, new MarketPrice(330000m));
        var repository = new FakeAssetRepository(asset);
        var finder = new AssetFinder(repository);
        var command = new UpdateMarketPriceCommand(asset.Id, new MarketPrice(400000m));
        var handler = new UpdateMarketPriceCommandHandler(repository, finder);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(repository.UpdateWasCalled);
        Assert.AreEqual(new MarketPrice(400000m), repository.UpdatedAsset!.MarketPrice);
        Assert.AreSame(asset, repository.UpdatedAsset);
    }
}