using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Exceptions;
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

    [TestMethod]
    [TestCategory("UpdateAssetCommandHandler tests")]
    public async Task Should_Update_With_Same_Ticker_And_Persist_Asset()
    {
        var asset = new Asset(new AssetName("Biitcoin"), new Ticker("BTC"),
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

    [TestMethod]
    [TestCategory("UpdateAssetCommandHandler tests")]
    public async Task Should_Return_Exception_When_Ticker_Is_In_Use_By_Other_Asset()
    {
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Stock, new MarketPrice(330000m));

        var wibx = new Asset(new AssetName("Wibx"), new Ticker("WBX"),
            EAssetType.Stock, new MarketPrice(0.07m));

        var repository = new FakeAssetRepository([bitcoin, wibx]);

        var finder = new AssetFinder(repository);

        //Ticker já em uso
        var command = new UpdateAssetCommand(wibx.Id, new AssetName("WIBX2"),
            new Ticker("BTC"), EAssetType.Cryptocurrency);

        var handler = new UpdateAssetCommandHandler(repository, finder);

        await Assert.ThrowsAsync<TickerAlreadyUsedException>(
    () => handler.HandleAsync(command, CancellationToken.None));
    }
}