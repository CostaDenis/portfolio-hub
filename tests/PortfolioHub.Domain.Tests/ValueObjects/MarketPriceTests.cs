using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

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
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Consider_Equal_Values_As_Equal()
        => Assert.AreEqual(new MarketPrice(_money), _marketPrice);

}
