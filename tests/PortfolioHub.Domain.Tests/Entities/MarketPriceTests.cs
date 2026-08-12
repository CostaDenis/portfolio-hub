using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.Entities;

[TestClass]
public class MarketPriceTests
{

    private readonly Money _money;
    private readonly MarketPrice _marketPrice;

    public MarketPriceTests()
    {
        _money = new(10.0m);
        _marketPrice = new(_money);
    }

    [TestMethod]
    [TestCategory("MarketPrice Tests")]
    public void Should_Return_Exception_When_MoneyValue_Is_Negative()
        => Assert.Throws<InvalidPriceException>
            (() => new MarketPrice(new Money(-10.0m)));

    [TestMethod]
    [TestCategory("MarketPrice Tests")]
    public void Should_Create_MarketPrice()
    {
        Assert.IsNotNull(_marketPrice);
        Assert.AreEqual(_money.Value, _marketPrice.Price.Value);
        Assert.AreNotEqual(Guid.Empty, _marketPrice.Id);
    }

    [TestMethod]
    [TestCategory("MarketPrice Tests")]
    public void Should_Return_Exception_When_Updating_Price_With_Negative_MoneyValue()
        => Assert.Throws<InvalidPriceException>
            (() => _marketPrice.UpdatePrice(new Money(-15.5m)));

    [TestMethod]
    [TestCategory("MarketPrice Tests")]
    public void Should_UpdatePrice()
    {
        DateTime dateTime = _marketPrice.LastUpdate;

        _marketPrice.UpdatePrice(new Money(15.5m));
        DateTime updatedDateTime = _marketPrice.LastUpdate;

        Assert.AreEqual(15.5m, _marketPrice.Price.Value);
        Assert.IsTrue(updatedDateTime >= dateTime);
    }

    [TestMethod]
    [TestCategory("MarketPrice Tests")]
    public void Should_Replace_Money_Instance_When_Updating_Price()
    {
        _marketPrice.UpdatePrice(15.0m);
        Money updatedMoney = _marketPrice.Price;

        Assert.AreEqual(10.0m, _money.Value);
        Assert.AreEqual(15.0m, updatedMoney.Value);
    }
}
