using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletTransactionsQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetWalletTransactionsQueryHandler tests")]
    public async Task Should_Return_Filtered_TransactionDTOs_Without_Changing_Wallet()
    {
        var wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        var walletRepository = new FakeWalletRepository(wallet);
        var walletFinder = new WalletFinder(walletRepository);

        var xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var btci11 = new Asset(new AssetName("BTCI11"), new Ticker("BTCI11"),
            EAssetType.RealStateFund, new MarketPrice(9m));

        wallet.BuyAsset(xpml11, new Quantity(2), new Money(105m));
        wallet.SellAsset(xpml11, new Quantity(1), new Money(110m));
        wallet.BuyAsset(btci11, new Quantity(5), new Money(9.05m));

        var query = new GetWalletTransactionsQuery(
            wallet.Id, xpml11.Id, ETransactionType.Buy, null, null);
        var handler = new GetWalletTransactionsQueryHandler(walletFinder);

        var result = await handler.HandleAsync(query, CancellationToken.None);
        var transaction = result.Single();

        Assert.HasCount(1, result);
        Assert.AreEqual(xpml11.Id, transaction.AssetId);
        Assert.AreEqual("XPML11", transaction.Ticker);
        Assert.AreEqual(ETransactionType.Buy.ToString(), transaction.Type);
        Assert.AreEqual(2m, transaction.Quantity);
        Assert.AreEqual(105m, transaction.UnitPrice);
        Assert.AreEqual(210m, transaction.Total);
        Assert.IsFalse(walletRepository.CreateWasCalled);
        Assert.IsFalse(walletRepository.UpdateWasCalled);
    }
}
