using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Infrastructure.Tests.Repositories;

[TestClass]
public class WalletRepositoryTests : InfrastructureTestBase
{
    private readonly Wallet _wallet;
    private readonly Asset _asset;

    public WalletRepositoryTests()
    {
        _wallet = new Wallet(new WalletName("FIIs"));
        _asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(109m));
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Create_And_Reload_Wallet()
    {
        await using (var setupContext = CreateDbContext())
        {
            var creationRepository = new WalletRepository(setupContext);
            await creationRepository.CreateWalletAsync(_wallet, CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new WalletRepository(context);

        var result = await repository.GetByIdAsync(_wallet.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(_wallet.Id, result.Id);
        Assert.AreEqual("FIIs", result.Name.Value);
        Assert.HasCount(0, result.Transactions);
        Assert.HasCount(0, result.Dividends);
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Return_Null_When_Wallet_Does_Not_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new WalletRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Persist_Updated_Wallet_Name()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Wallets.Add(_wallet);
            await setupContext.SaveChangesAsync();
        }

        var newWalletName = new WalletName("Fundos Imobiliários");

        await using (var updateContext = CreateDbContext())
        {
            var repository = new WalletRepository(updateContext);
            var storedWallet = await repository.GetByIdAsync(_wallet.Id, CancellationToken.None);

            Assert.IsNotNull(storedWallet);

            storedWallet.UpdateName(newWalletName);
            await repository.UpdateAsync(storedWallet, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new WalletRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_wallet.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Fundos Imobiliários", result.Name.Value);
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Load_Transactions_And_Their_Assets()
    {
        _wallet.BuyAsset(_asset, new Quantity(10), new Money(105m));
        _wallet.SellAsset(_asset, new Quantity(3), new Money(112m));

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Wallets.Add(_wallet);
            await setupContext.SaveChangesAsync();
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new WalletRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_wallet.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.HasCount(2, result.Transactions);
        var buy = result.Transactions.Single(transaction => transaction.Type == ETransactionType.Buy);
        var sell = result.Transactions.Single(transaction => transaction.Type == ETransactionType.Sell);
        Assert.AreEqual(10m, buy.Quantity.Value);
        Assert.AreEqual(105m, buy.UnitPrice.Value);
        Assert.AreEqual(3m, sell.Quantity.Value);
        Assert.AreEqual(112m, sell.UnitPrice.Value);
        foreach (var transaction in result.Transactions)
        {
            Assert.AreEqual(_asset.Id, transaction.Asset.Id);
            Assert.AreEqual("XPML11", transaction.Asset.Ticker.Value);
            Assert.AreEqual(109m, transaction.Asset.MarketPrice.Price.Value);
        }
        Assert.AreEqual(new Quantity(7), result.GetCurrentQuantity(_asset));

    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Load_Dividends_And_Their_Assets()
    {
        _wallet.BuyAsset(_asset, new Quantity(10), new Money(105m));
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
        _wallet.ReceiveDividend(_asset, new Money(0.92m), date);

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Wallets.Add(_wallet);
            await setupContext.SaveChangesAsync();
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new WalletRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_wallet.Id, CancellationToken.None);
        Money dividendCorrectResult = 0.92m * new Quantity(10);

        Assert.IsNotNull(result);
        Assert.AreEqual(new Quantity(10), result.GetCurrentQuantity(_asset));
        Assert.AreEqual(dividendCorrectResult, result.GetTotalDividendsByAsset(_asset));
        Assert.HasCount(1, result.Dividends);
        var dividend = result.Dividends.Single();
        Assert.AreEqual(_wallet.Dividends.Single().Id, dividend.Id);
        Assert.AreEqual(_asset.Id, dividend.Asset.Id);
        Assert.AreEqual("XPML11", dividend.Asset.Ticker.Value);
        Assert.AreEqual(10m, dividend.Quantity.Value);
        Assert.AreEqual(0.92m, dividend.ValuePerShare.Value);
        Assert.AreEqual(date, dividend.Date);
        Assert.AreEqual(9.2m, dividend.Total.Value);
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Persist_New_Transaction_When_Updating_Wallet()
    {
        _wallet.BuyAsset(_asset, new Quantity(10), new Money(105m));
        var originalTransactionId = _wallet.Transactions.Single().Id;

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Wallets.Add(_wallet);
            await setupContext.SaveChangesAsync();
        }

        Guid newTransactionId;
        await using (var updateContext = CreateDbContext())
        {
            var repository = new WalletRepository(updateContext);
            var storedWallet = await repository.GetByIdAsync(_wallet.Id, CancellationToken.None);
            Assert.IsNotNull(storedWallet);
            var storedAsset = storedWallet.Transactions.Single().Asset;

            storedWallet.BuyAsset(storedAsset, new Quantity(2), new Money(110m));
            newTransactionId = storedWallet.Transactions.Single(t => t.Id != originalTransactionId).Id;
            await repository.UpdateAsync(storedWallet, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new WalletRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_wallet.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.HasCount(2, result.Transactions);
        var original = result.Transactions.Single(t => t.Id == originalTransactionId);
        Assert.AreEqual(10m, original.Quantity.Value);
        Assert.AreEqual(105m, original.UnitPrice.Value);
        var added = result.Transactions.Single(t => t.Id == newTransactionId);
        Assert.AreEqual(ETransactionType.Buy, added.Type);
        Assert.AreEqual(_asset.Id, added.Asset.Id);
        Assert.AreEqual(2m, added.Quantity.Value);
        Assert.AreEqual(110m, added.UnitPrice.Value);
        Assert.AreEqual(12m, result.GetCurrentQuantity(_asset).Value);
    }

    [TestMethod]
    [TestCategory("WalletRepository tests")]
    public async Task Should_Persist_New_Dividend_When_Updating_Wallet()
    {
        var firstDate = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc);
        var secondDate = firstDate.AddMonths(1);
        _wallet.BuyAsset(_asset, new Quantity(10), new Money(105m));
        _wallet.ReceiveDividend(_asset, new Money(0.90m), firstDate);
        var originalDividendId = _wallet.Dividends.Single().Id;

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Wallets.Add(_wallet);
            await setupContext.SaveChangesAsync();
        }

        Guid newDividendId;
        await using (var updateContext = CreateDbContext())
        {
            var repository = new WalletRepository(updateContext);
            var storedWallet = await repository.GetByIdAsync(_wallet.Id, CancellationToken.None);
            Assert.IsNotNull(storedWallet);
            var storedAsset = storedWallet.Transactions.Single().Asset;

            storedWallet.ReceiveDividend(storedAsset, new Money(0.92m), secondDate);
            newDividendId = storedWallet.Dividends.Single(d => d.Id != originalDividendId).Id;
            await repository.UpdateAsync(storedWallet, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new WalletRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_wallet.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.HasCount(2, result.Dividends);
        var original = result.Dividends.Single(d => d.Id == originalDividendId);
        Assert.AreEqual(0.90m, original.ValuePerShare.Value);
        Assert.AreEqual(firstDate, original.Date);
        var added = result.Dividends.Single(d => d.Id == newDividendId);
        Assert.AreEqual(_asset.Id, added.Asset.Id);
        Assert.AreEqual("XPML11", added.Asset.Ticker.Value);
        Assert.AreEqual(10m, added.Quantity.Value);
        Assert.AreEqual(0.92m, added.ValuePerShare.Value);
        Assert.AreEqual(secondDate, added.Date);
        Assert.AreEqual(9.2m, added.Total.Value);
        Assert.AreEqual(18.2m, result.GetTotalDividends().Value);
        Assert.HasCount(1, result.Transactions);
        Assert.AreEqual(10m, result.GetCurrentQuantity(_asset).Value);
    }

}
