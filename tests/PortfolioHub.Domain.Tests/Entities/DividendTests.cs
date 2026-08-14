using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.Entities;

[TestClass]
public class DividendTests
{
    private readonly Asset _asset;
    private readonly DateTime _date;
    private readonly Dividend _dividend;

    public DividendTests()
    {
        _asset = new(new AssetName("IRIDIUM FUNDO"), new Ticker("IRIM11"),
                EAssetType.RealStateFund, new MarketPrice(60.65m));
        _date = new DateTime(2026, 8, 13, 12, 0, 0, DateTimeKind.Utc);
        _dividend = new(_asset, new Quantity(3), new Money(0.77m), _date);
    }

    [TestMethod]
    [TestCategory("Dividend Tests")]
    public void Should_Return_Exception_When_Quantity_Is_Zero()
        => Assert.Throws<InvalidQuantityException>(() => new Dividend(
            new Asset(new AssetName("IRIDIUM FUNDO"),
                new Ticker("IRIM11"),
                EAssetType.RealStateFund,
                new MarketPrice(60.65m)
            ),
            new Quantity(0),
            new Money(0.77m),
            DateTime.UtcNow));

    [TestMethod]
    [TestCategory("Dividend Tests")]
    public void Should_Return_Exception_When_ValuePerShare_Is_Negative()
    => Assert.Throws<InvalidPriceException>(() => new Dividend(
            new Asset(new AssetName("IRIDIUM FUNDO"),
                new Ticker("IRIM11"),
                EAssetType.RealStateFund,
                new MarketPrice(60.65m)
            ),
            new Quantity(3),
            new Money(-0.77m),
            DateTime.UtcNow));

    [TestMethod]
    [TestCategory("Dividend Tests")]
    public void Should_Return_Exception_When_ValuePerShare_Is_Zero()
    => Assert.Throws<InvalidPriceException>(() => new Dividend(
            new Asset(new AssetName("IRIDIUM FUNDO"),
                new Ticker("IRIM11"),
                EAssetType.RealStateFund,
                new MarketPrice(60.65m)
            ),
            new Quantity(3),
            new Money(0),
            DateTime.UtcNow));

    [TestMethod]
    [TestCategory("Dividend Tests")]
    public void Should_Create_Dividend()
    {
        Assert.IsNotNull(_dividend);
        Assert.AreNotEqual(Guid.Empty, _dividend.Id);
        Assert.AreEqual("IRIDIUM FUNDO", _dividend.Asset.Name.Value);
        Assert.AreEqual(new Quantity(3), _dividend.Quantity);
        Assert.AreEqual(new Money(0.77m), _dividend.ValuePerShare);
        Assert.AreEqual(_date, _dividend.Date);
    }

    [TestMethod]
    [TestCategory("Dividend Tests")]
    public void Should_Return_Success_When_TotalPropertie_Is_Correct()
    {
        var correctResult = _dividend.Quantity.Value * _dividend.ValuePerShare;

        Assert.AreEqual(correctResult, _dividend.Total.Value);
    }
}