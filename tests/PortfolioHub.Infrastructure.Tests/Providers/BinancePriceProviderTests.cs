using System.Net;
using PortfolioHub.Infrastructure.Providers;

namespace PortfolioHub.Infrastructure.Tests.Providers;

[TestClass]
public class BinancePriceProviderTests
{
    //formato em que a Binance responde
    private const string BinanceResponse = """
        [
            { "symbol": "BTCBRL", "price": "367826.00000000" },
            { "symbol": "ETHBRL", "price": "10280.44000000" },
            { "symbol": "ETCBRL", "price": "0.00000000" },
            { "symbol": "BTCUSDT", "price": "68000.00000000" }
        ]
        """;

    private static (BinancePriceProvider Provider, StubHttpMessageHandler Handler) CreateProvider(
        string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new StubHttpMessageHandler(json, statusCode);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.binance.com/") };

        return (new BinancePriceProvider(client), handler);
    }

    [TestMethod]
    [TestCategory("BinancePriceProvider Tests")]
    public async Task Should_Return_Prices_By_Original_Ticker()
    {
        var (provider, handler) = CreateProvider(BinanceResponse);

        var result = await provider.GetPricesAsync(["BTC", "ETH"], CancellationToken.None);

        Assert.HasCount(2, result);
        Assert.AreEqual(367826m, result["BTC"]);
        Assert.AreEqual(10280.44m, result["ETH"]);
        Assert.AreEqual("https://api.binance.com/api/v3/ticker/price",
            handler.LastRequest?.RequestUri?.ToString());
    }

    [TestMethod]
    [TestCategory("BinancePriceProvider Tests")]
    public async Task Should_Ignore_Pairs_With_Zero_Price()
    {
        var (provider, handler) = CreateProvider(BinanceResponse);
        var result = await provider.GetPricesAsync(["ETC"], CancellationToken.None);

        Assert.HasCount(0, result);
    }

    [TestMethod]
    [TestCategory("BinancePriceProvider Tests")]
    public async Task Should_Ignore_Tickers_Without_Brl_Pair()
    {
        var (provider, handler) = CreateProvider(BinanceResponse);
        var result = await provider.GetPricesAsync(["XYZ"], CancellationToken.None);

        Assert.HasCount(0, result);
    }

    [TestMethod]
    [TestCategory("BinancePriceProvider Tests")]
    public async Task Should_Throw_When_Binance_Is_Unavailable()
    {
        //Binance fora do ar
        var (provider, _) = CreateProvider("", HttpStatusCode.InternalServerError);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => provider.GetPricesAsync(["BTC"], CancellationToken.None));
    }
}
