using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Assets;

[TestClass]
public class UpdateAssetCommandHandlerTests
{
    [TestMethod]
    [TestCategory("UpdateAssetCommandHandler tests")]
    public async Task Should_Update_And_Persist_Asset()
    {
        var asset = new Asset(new AssetName("Biitcoin"), new Ticker("BTTC"),
            EAssetType.Stock, new MarketPrice(330000m));

        var repository = new FakeAssetRepository(asset);
        var finder = new AssetFinder(repository);
        var command = new UpdateAssetCommand(asset.Id, new AssetName("Bitcoin"),
            new Ticker("BTC"), EAssetType.Cryptocurrency);

        var handler = new UpdateAssetCommandHandler(repository, finder);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(repository.UpdateWasCalled);
        Assert.AreSame(asset, repository.UpdatedAsset);
        Assert.AreEqual(new AssetName("Bitcoin"), repository.UpdatedAsset!.Name);
        Assert.AreEqual(new Ticker("BTC"), repository.UpdatedAsset!.Ticker);
        Assert.AreEqual(EAssetType.Cryptocurrency, repository.UpdatedAsset!.Type);
    }
}