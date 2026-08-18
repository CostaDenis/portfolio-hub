using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletDividendsQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetWalletDividendsQueryHandler tests")]
    public async Task Should_Return_WalletDividendsDTO_Without_Changing_Wallet()
    {
        var wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        var walletRepository = new FakeWalletRepository(wallet);
        var walletFinder = new WalletFinder(walletRepository);

        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var date = new DateTime(2026, 8, 18, 12, 0, 0, DateTimeKind.Utc);

        wallet.BuyAsset(asset, new Quantity(2), new Money(105m));
        wallet.ReceiveDividend(asset, new Money(0.91m), date);

        var query = new GetWalletDividendsQuery(wallet.Id);
        var handler = new GetWalletDividendsQueryHandler(walletFinder);

        var result = await handler.HandleAsync(query, CancellationToken.None);
        var dividend = result.Dividends.Single();

        Assert.AreEqual(1.82m, result.TotalReceived);
        Assert.HasCount(1, result.Dividends);
        Assert.AreEqual(asset.Id, dividend.AssetId);
        Assert.AreEqual(asset.Ticker.Value, dividend.Ticker);
        Assert.AreEqual(2m, dividend.Quantity);
        Assert.AreEqual(0.91m, dividend.ValuePerShare);
        Assert.AreEqual(date, dividend.Date);
        Assert.AreEqual(1.82m, dividend.Total);
        Assert.IsFalse(walletRepository.CreateWasCalled);
        Assert.IsFalse(walletRepository.UpdateWasCalled);
    }
}
