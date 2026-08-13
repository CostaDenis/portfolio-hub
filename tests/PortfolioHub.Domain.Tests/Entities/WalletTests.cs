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
    private readonly Asset _asset;

    public WalletTests()
    {
        _wallet = new(new WalletName("Fundos Imobiliários"));
        _asset = new(new AssetName("XP Malls"), new Ticker("XPML11"), Enums.EAssetType.RealStateFund, new MarketPrice(105.0m));
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
    [TestCategory("MarketPrice Tests")]
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
        _wallet.BuyAsset(_asset, 2, 105.0m);
        Assert.IsTrue(_wallet.ContainsAsset(_asset));
    }

    [TestMethod]
    public void Should_Return_Current_Quantity_After_Buy()
    {
        _wallet.BuyAsset(_asset, 5, 105m);

        Assert.AreEqual(5, _wallet.GetCurrentQuantity(_asset).Value);
    }

    [TestMethod]
    public void Should_Return_Current_Quantity_After_Buy_And_Sell()
    {
        _wallet.BuyAsset(_asset, 10, 105m);
        _wallet.SellAsset(_asset, 4, 110m);

        Assert.AreEqual(6, _wallet.GetCurrentQuantity(_asset).Value);
    }

    [TestMethod]
    public void Should_Return_Zero_When_Asset_Is_Not_In_Wallet()
    {
        Assert.AreEqual(0, _wallet.GetCurrentQuantity(_asset).Value);
    }

    [TestMethod]
    public void Should_Return_False_When_Wallet_Does_Not_Contain_Asset()
    {
        Assert.IsFalse(_wallet.ContainsAsset(_asset));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Return_Exception_When_Wallet_Not_Has_Enough_Quantity_To_Sell()
    {
        _wallet.BuyAsset(_asset, 2, 105.0m);

        Assert.Throws<InsufficientBalance>(() => _wallet.SellAsset(_asset, 5, 106.0m));
    }

    [TestMethod]
    [TestCategory("Wallet Tests")]
    public void Should_Sell_Asset()
    {
        _wallet.BuyAsset(_asset, 2, 105.0m);
        _wallet.SellAsset(_asset, 1, 106.0m);

        Assert.AreEqual(1, _wallet.GetCurrentQuantity(_asset).Value);
    }

    [TestMethod]
    public void Should_Not_Contain_Asset_After_Selling_All_Quantity()
    {
        _wallet.BuyAsset(_asset, 5, 105m);
        _wallet.SellAsset(_asset, 5, 110m);

        Assert.IsFalse(_wallet.ContainsAsset(_asset));
    }

    [TestMethod]
    public void Should_Return_Only_Transactions_From_Asset()
    {
        var anotherAsset = new Asset(
            new AssetName("Bitcoin"),
            new Ticker("BTC"),
            EAssetType.Cryptocurrency,
            new MarketPrice(320000m));

        _wallet.BuyAsset(_asset, 2, 105m);
        _wallet.BuyAsset(anotherAsset, 1, 320000m);

        var transactions = _wallet.GetTransactions(_asset);

        Assert.HasCount(1, transactions);
        Assert.AreSame(_asset, transactions.First().Asset);
    }
}