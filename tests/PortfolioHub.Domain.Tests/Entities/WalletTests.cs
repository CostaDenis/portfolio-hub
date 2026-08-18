using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.Entities;

[TestClass]
public class WalletTests
{

    private readonly Wallet _wallet;
    private readonly Asset _xpml11;
    private readonly Asset _btci11;

    public WalletTests()
    {
        _wallet = new(new WalletName("Fundos Imobiliários"));

        _xpml11 = new(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(105.0m));

        _btci11 = new Asset(new AssetName("BTG Pactual"), new Ticker("BTCI11"),
            EAssetType.RealStateFund, new MarketPrice(9.1m));

    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Create_Wallet()
    {
        Assert.IsNotNull(_wallet);
        Assert.AreEqual("Fundos Imobiliários", _wallet.Name.Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Update_WalletName()
    {
        _wallet.UpdateName(new WalletName("Ações"));
        Assert.AreEqual("Ações", _wallet.Name.Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Replace_WalletName_Instance_When_Updating_WalletName()
    {
        string walletName = _wallet.Name;

        _wallet.UpdateName(new WalletName("Ações"));
        string newWalletName = _wallet.Name;

        Assert.AreEqual("Fundos Imobiliários", walletName);
        Assert.AreEqual("Ações", newWalletName);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Buy_Asset()
    {
        _wallet.BuyAsset(_xpml11, 2, 105.0m);
        Assert.IsTrue(_wallet.ContainsAsset(_xpml11));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Current_Quantity_After_Buy()
    {
        _wallet.BuyAsset(_xpml11, 5, 105m);

        Assert.AreEqual(5, _wallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Current_Quantity_After_Buy_And_Sell()
    {
        _wallet.BuyAsset(_xpml11, 10, 105m);
        _wallet.SellAsset(_xpml11, 4, 110m);

        Assert.AreEqual(6, _wallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Zero_When_Asset_Is_Not_In_Wallet()
        => Assert.AreEqual(0, _wallet.GetCurrentQuantity(_xpml11).Value);

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_False_When_Wallet_Does_Not_Contain_Asset()
        => Assert.IsFalse(_wallet.ContainsAsset(_xpml11));

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Exception_When_Wallet_Not_Has_Enough_Quantity_To_Sell()
    {
        _wallet.BuyAsset(_xpml11, 2, 105.0m);

        Assert.Throws<InsufficientBalanceException>(() => _wallet.SellAsset(_xpml11, 5, 106.0m));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Sell_Asset()
    {
        _wallet.BuyAsset(_xpml11, 2, 105.0m);
        _wallet.SellAsset(_xpml11, 1, 106.0m);

        Assert.AreEqual(1, _wallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Not_Contain_Asset_After_Selling_All_Quantity()
    {
        _wallet.BuyAsset(_xpml11, 5, 105m);
        _wallet.SellAsset(_xpml11, 5, 110m);

        Assert.IsFalse(_wallet.ContainsAsset(_xpml11));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Only_Transactions_From_Asset()
    {
        var anotherAsset = new Asset(
            new AssetName("Bitcoin"),
            new Ticker("BTC"),
            EAssetType.Cryptocurrency,
            new MarketPrice(320000m));

        _wallet.BuyAsset(_xpml11, 2, 105m);
        _wallet.BuyAsset(anotherAsset, 1, 320000m);

        var transactions = _wallet.GetTransactions(_xpml11);

        Assert.HasCount(1, transactions);
        Assert.AreSame(_xpml11, transactions.First().Asset);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Receive_Dividend()
    {
        _wallet.BuyAsset(_xpml11, 2, 105.0m);
        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);

        Assert.AreEqual(0.91m * 2, _wallet.GetTotalDividendsByAsset(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Dividends_By_Asset()
    {
        _wallet.BuyAsset(_xpml11, 2, 105m);
        _wallet.BuyAsset(_btci11, 5, 9.1m);

        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);
        _wallet.ReceiveDividend(_btci11, 0.09m, DateTime.UtcNow);

        var xpml11Dividends = _wallet.GetDividendsByAsset(_xpml11);
        var btci11Dividends = _wallet.GetDividendsByAsset(_btci11);

        Assert.HasCount(1, xpml11Dividends);
        Assert.HasCount(1, btci11Dividends);

        Assert.AreSame(_xpml11, xpml11Dividends.Single().Asset);
        Assert.AreSame(_btci11, btci11Dividends.Single().Asset);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Success_When_GetTotalDividends_Is_Correct()
    {
        _wallet.BuyAsset(_xpml11, 2, 105m);
        _wallet.BuyAsset(_btci11, 5, 9.1m);

        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);
        _wallet.ReceiveDividend(_btci11, 0.09m, DateTime.UtcNow);

        var totalDividends = _wallet.GetTotalDividends();
        Money correctResult = (0.91m * 2) + (0.09m * 5);

        Assert.AreEqual(correctResult, totalDividends);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Not_Change_Position_When_ReceiveDividend()
    {
        _wallet.BuyAsset(_xpml11, 2, 105m);
        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);
        var xpml11Count = _wallet.GetCurrentQuantity(_xpml11);

        Assert.AreEqual(new Quantity(2), xpml11Count);
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Exception_When_Has_No_Asset_To_ReceiveDividend()
        => Assert.Throws<InsufficientAssetQuantityException>
            (() => _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow));

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Success_When_TotalDividend_Is_Valid_With_Partial_Sell()
    {
        _wallet.BuyAsset(_xpml11, 8, 105m);
        _wallet.SellAsset(_xpml11, 2, 103m);

        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);
        Money correctResult = 6 * 0.91m;

        Assert.AreEqual(correctResult, _wallet.GetTotalDividendsByAsset(_xpml11));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Success_When_GetTotalDividendsByAsset_Is_Correct()
    {
        _wallet.BuyAsset(_xpml11, 2, 105m);
        _wallet.ReceiveDividend(_xpml11, 0.91m, DateTime.UtcNow);
        _wallet.ReceiveDividend(_xpml11, 0.94m, DateTime.UtcNow);

        Money correctResult = (0.91m * 2) + (0.94m * 2);

        Assert.AreEqual(correctResult, _wallet.GetTotalDividendsByAsset(_xpml11));
    }

}
