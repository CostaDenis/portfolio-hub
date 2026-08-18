using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletPositionQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetWalletPositionQueryHandler tests")]
    public async Task Should_Return_WalletPositionDTOs_Without_Changing_Wallet()
    {
        var wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        var walletRepository = new FakeWalletRepository(wallet);
        var walletFinder = new WalletFinder(walletRepository);

        var xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var btci11 = new Asset(new AssetName("BTG fundos"), new Ticker("BTCI11"),
            EAssetType.RealStateFund, new MarketPrice(9m));

        wallet.BuyAsset(xpml11, new Quantity(2), new Money(105m));
        wallet.BuyAsset(btci11, new Quantity(2), new Money(9.05m));

        var query = new GetWalletPositionQuery(wallet.Id);
        var handler = new GetWalletPositionQueryHandler(walletFinder);

        var result = await handler.HandleAsync(query, CancellationToken.None);
        var xpml11Position = result.Single(position => position.AssetId == xpml11.Id);
        var btci11Position = result.Single(position => position.AssetId == btci11.Id);

        Assert.HasCount(2, result);
        Assert.AreEqual("XPML11", xpml11Position.Ticker);
        Assert.AreEqual(2m, xpml11Position.Quantity);
        Assert.AreEqual(110m, xpml11Position.MarketPrice);
        Assert.AreEqual("BTCI11", btci11Position.Ticker);
        Assert.AreEqual(2m, btci11Position.Quantity);
        Assert.AreEqual(9m, btci11Position.MarketPrice);
        Assert.IsFalse(walletRepository.CreateWasCalled);
        Assert.IsFalse(walletRepository.UpdateWasCalled);
    }
}
