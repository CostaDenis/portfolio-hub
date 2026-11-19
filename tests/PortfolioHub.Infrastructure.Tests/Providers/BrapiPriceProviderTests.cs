using Microsoft.Extensions.Time.Testing;
using PortfolioHub.Infrastructure.Providers;

namespace PortfolioHub.Infrastructure.Tests.Providers;

[TestClass]
public class BrapiPriceProviderTests
{

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public void Should_Be_Available_During_Trading_Hours()
    {
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 18, 17, 0, 0, TimeSpan.Zero));
        var provider = new BrapiPriceProvider(new HttpClient(), time);

        Assert.IsTrue(provider.IsAvailableNow());
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public void Should_Not_Be_Available_After_Market_Close()
    {
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 18, 23, 0, 0, TimeSpan.Zero));
        var provider = new BrapiPriceProvider(new HttpClient(), time);

        Assert.IsFalse(provider.IsAvailableNow());
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public void Should_Not_Be_Available_On_Weekends()
    {
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 21, 17, 0, 0, TimeSpan.Zero));
        var provider = new BrapiPriceProvider(new HttpClient(), time);

        Assert.IsFalse(provider.IsAvailableNow());
    }


    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public void Should_Be_Available_At_Market_Close()
    {
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 18, 21, 30, 0, TimeSpan.Zero));
        var provider = new BrapiPriceProvider(new HttpClient(), time);

        Assert.IsTrue(provider.IsAvailableNow());
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public void Should_Not_Be_Available_Before_Market_Open()
    {
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 18, 12, 59, 0, TimeSpan.Zero));
        var provider = new BrapiPriceProvider(new HttpClient(), time);

        Assert.IsFalse(provider.IsAvailableNow());
    }
}
