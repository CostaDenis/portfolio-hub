using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletsQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetWalletsQueryHandler tests")]
    public async Task Should_Return_Wallet_Summaries_Ordered_By_Name()
    {
        var xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var petr4 = new Asset(new AssetName("Petrobras"), new Ticker("PETR4"),
            EAssetType.Stock, new MarketPrice(38m));

        var fiis = new Wallet(new WalletName("FIIs"));
        fiis.BuyAsset(xpml11, 10, 105m);

        var stocks = new Wallet(new WalletName("Ações"));
        stocks.BuyAsset(petr4, 100, 36m);
        stocks.BuyAsset(xpml11, 2, 100m);

        var empty = new Wallet(new WalletName("Reserva"));

        var repository = new FakeWalletRepository([fiis, stocks, empty]);
        var handler = new GetWalletsQueryHandler(repository);

        var result = (await handler.HandleAsync(CancellationToken.None)).ToList();

        Assert.HasCount(3, result);
        CollectionAssert.AreEqual(
            new[] { "Ações", "FIIs", "Reserva" }, result.Select(wallet => wallet.Name).ToArray());

        Assert.AreEqual(stocks.Id, result[0].WalletId);
        Assert.AreEqual((100 * 38m) + (2 * 110m), result[0].NetWorth);
        Assert.AreEqual(2, result[0].AssetCount);

        Assert.AreEqual(10 * 110m, result[1].NetWorth);
        Assert.AreEqual(1, result[1].AssetCount);

        Assert.AreEqual(0m, result[2].NetWorth);
        Assert.AreEqual(0, result[2].AssetCount);
    }

    [TestMethod]
    [TestCategory("GetWalletsQueryHandler tests")]
    public async Task Should_Return_Empty_List_When_No_Wallets_Exist()
    {
        var handler = new GetWalletsQueryHandler(new FakeWalletRepository());

        var result = await handler.HandleAsync(CancellationToken.None);

        Assert.HasCount(0, result);
    }
}
