using System.Globalization;
using System.Net;
using Microsoft.Extensions.Time.Testing;
using PortfolioHub.Infrastructure.Providers;

namespace PortfolioHub.Infrastructure.Tests.Providers;

[TestClass]
public class BrapiPriceProviderTests
{

    private static (BrapiPriceProvider Provider, StubHttpMessageHandler Handler) CreateProvider(
        Func<string, HttpResponseMessage> respondByTicker)
    {
        // https://brapi.dev/api/quote/PETR4 → último segmento da URL = "PETR4"
        var handler = new StubHttpMessageHandler(request => respondByTicker(request.RequestUri!.Segments[^1]));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://brapi.dev/") };

        return (new BrapiPriceProvider(client, TimeProvider.System), handler);
    }

    //formato da resposta do Brapi
    private static HttpResponseMessage Quote(string ticker, decimal price)
        => StubHttpMessageHandler.JsonResponse($$"""
            { "results": [ { "symbol": "{{ticker}}", "regularMarketPrice": {{price.ToString(CultureInfo.InvariantCulture)}} } ] }
            """);

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public async Task Should_Return_Prices_For_Each_Ticker()
    {
        var (provider, handler) = CreateProvider(ticker => ticker switch
        {
            "PETR4" => Quote("PETR4", 43.17m),
            "XPML11" => Quote("XPML11", 105.12m),
            _ => StubHttpMessageHandler.JsonResponse("", HttpStatusCode.NotFound),
        });

        var result = await provider.GetPricesAsync(["PETR4", "XPML11"], CancellationToken.None);

        Assert.HasCount(2, result);
        Assert.AreEqual(43.17m, result["PETR4"]);
        Assert.AreEqual(105.12m, result["XPML11"]);

        Assert.HasCount(2, handler.Requests);
        Assert.AreEqual("https://brapi.dev/api/quote/PETR4", handler.Requests[0].RequestUri!.ToString());
        Assert.AreEqual("https://brapi.dev/api/quote/XPML11", handler.Requests[1].RequestUri!.ToString());
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public async Task Should_Skip_Ticker_Not_Found()
    {
        var (provider, handler) = CreateProvider(ticker => ticker switch
        {
            "PETR4" => Quote("PETR4", 43.17m),
            _ => StubHttpMessageHandler.JsonResponse("", HttpStatusCode.NotFound),
        });

        var result = await provider.GetPricesAsync(["XXXX1", "PETR4"], CancellationToken.None);

        Assert.HasCount(1, result);
        Assert.AreEqual(43.17m, result["PETR4"]);
        Assert.HasCount(2, handler.Requests);
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public async Task Should_Throw_When_Token_Is_Invalid()
    {
        var (provider, handler) = CreateProvider(ticker =>
        ticker switch
        {
            _ => StubHttpMessageHandler.JsonResponse("", HttpStatusCode.Unauthorized)
        });

        await Assert.ThrowsAsync<HttpRequestException>(
            () => provider.GetPricesAsync(["BBAS3", "PETR4"], CancellationToken.None)
        );

        Assert.HasCount(1, handler.Requests);
    }

    [TestMethod]
    [TestCategory("BrapiPriceProvider Tests")]
    public async Task Should_Skip_Ticker_With_Zero_Price()
    {
        var (provider, handler) = CreateProvider(ticker =>
        ticker switch
        {
            "PETR4" => Quote("PETR4", 0m),
            _ => StubHttpMessageHandler.JsonResponse("", HttpStatusCode.Unauthorized)
        });

        var result = await provider.GetPricesAsync(["PETR4"], CancellationToken.None);

        Assert.HasCount(0, result);
    }

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
