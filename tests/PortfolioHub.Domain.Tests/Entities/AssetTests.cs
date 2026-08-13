using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.Entities;

[TestClass]
public class AssetTests
{

    private readonly AssetName _assetName;
    private readonly Ticker _ticker;
    private readonly EAssetType _type;
    private readonly MarketPrice _marketPrice;
    private readonly Asset _asset;

    public AssetTests()
    {
        _assetName = new("Bitcoin");
        _ticker = new("btc");
        _type = EAssetType.Cryptocurrency;
        _marketPrice = new(320000.0m);
        _asset = new(_assetName, _ticker, _type, _marketPrice);
    }

    [TestMethod]
    [TestCategory("Asset Tests")]
    public void Should_Create_Asset()
    {
        Assert.IsNotNull(_asset);
        Assert.AreNotEqual(Guid.Empty, _asset.Id);
        Assert.AreEqual("Bitcoin", _asset.Name.Value);
        Assert.AreEqual("BTC", _asset.Ticker.Value);
        Assert.AreEqual(EAssetType.Cryptocurrency, _asset.Type);
    }

    [TestMethod]
    [TestCategory("Asset Tests")]
    public void Should_Update_Asset_Name()
    {
        _asset.UpdateName(new AssetName("Ethereum"));
        Assert.AreEqual("Ethereum", _asset.Name.Value);
    }

    [TestMethod]
    [TestCategory("Asset Tests")]
    public void Should_Update_Ticker()
    {
        _asset.UpdateTicker(new Ticker("eth"));
        Assert.AreEqual("ETH", _asset.Ticker.Value);
    }

    [TestMethod]
    [TestCategory("Asset Tests")]
    public void Should_Update_Type()
    {
        _asset.UpdateType(EAssetType.RealStateFund);
        Assert.AreEqual(EAssetType.RealStateFund, _asset.Type);
    }

    [TestMethod]
    [TestCategory("Asset Tests")]
    public void Should_Update_MarketPrice()
    {
        _asset.UpdateMarketPrice(new MarketPrice(350000m));
        Assert.AreEqual(350000m, _asset.MarketPrice.Price.Value);
    }

}
