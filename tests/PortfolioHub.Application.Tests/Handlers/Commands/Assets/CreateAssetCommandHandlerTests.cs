using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Assets;

[TestClass]
public class CreateAssetCommandHandlerTests
{

    [TestMethod]
    [TestCategory("CreateAssetCommandHandler tests")]
    public async Task Should_Return_Exception_When_Ticker_Already_Registred()
    {
        var command = new CreateAssetCommand
           (new AssetName("BTG Pactual Credito"), new Ticker("BTCI11"),
           EAssetType.RealStateFund, new MarketPrice(9.0M));
        var asset = new Asset(command.AssetName, command.Ticker, command.Type, command.MarketPrice);
        var repository = new FakeAssetRepository(asset);

        var handler = new CreateAssetCommandHandler(repository);

        await Assert.ThrowsAsync<TickerAlreadyUsedException>
            (async () => await handler.HandleAsync(command, CancellationToken.None));
        Assert.IsFalse(repository.CreateWasCalled);
    }

    [TestMethod]
    [TestCategory("CreateAssetCommandHandler tests")]
    public async Task Should_Create_Asset()
    {
        var command = new CreateAssetCommand
            (new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(97.0M));
        var asset = new Asset(command.AssetName, command.Ticker, command.Type, command.MarketPrice);
        var repository = new FakeAssetRepository();

        var handler = new CreateAssetCommandHandler(repository);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(repository.CreateWasCalled);
        Assert.IsNotNull(repository.CreatedAsset);
        Assert.AreEqual(new AssetName("XP Malls"), repository.CreatedAsset.Name);
        Assert.AreEqual(new Ticker("XPML11"), repository.CreatedAsset.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, repository.CreatedAsset.Type);
        Assert.AreEqual(97m, repository.CreatedAsset.MarketPrice.Price.Value);
    }
}
