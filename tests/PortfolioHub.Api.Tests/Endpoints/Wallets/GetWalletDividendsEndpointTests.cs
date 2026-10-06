using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletDividendsEndpointTests : ApiTestBase
{

    [TestMethod]
    [TestCategory("GetWalletDividendsEndpoint tests")]
    public async Task Should_Return_Ok_When_Wallet_Exist()
    {
        var wallet = new Wallet(new WalletName("Ações"));
        var asset = new Asset
            (new AssetName("Banco do Brasil SA"), new Ticker("BBAS3"), EAssetType.Stock, new MarketPrice(23m));

        wallet.BuyAsset(asset, new Quantity(10m), new Money(23m));
        wallet.ReceiveDividend(asset, new Money(0.69m), DateTime.UtcNow);

        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.Assets.AddAsync(asset, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}/dividends", CancellationToken.None);
        var json = JsonDocument
            .Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;
        var correctTotalDividends = 0.69m * 10m;
        var dividends = body.GetProperty("dividends");
        var dividend = dividends.EnumerateArray().Single();
        var expectedDividend = wallet.Dividends.Single();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(correctTotalDividends, body.GetProperty("totalReceived").GetDecimal());

        Assert.AreEqual(JsonValueKind.Array, dividends.ValueKind);
        Assert.AreEqual(1, dividends.GetArrayLength());

        Assert.AreEqual(expectedDividend.Id, dividend.GetProperty("dividendId").GetGuid());
        Assert.AreEqual(asset.Id, dividend.GetProperty("assetId").GetGuid());
        Assert.AreEqual("BBAS3", dividend.GetProperty("ticker").GetString());
        Assert.AreEqual(10m, dividend.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(0.69m, dividend.GetProperty("valuePerShare").GetDecimal());
        Assert.AreEqual(expectedDividend.Date, dividend.GetProperty("date").GetDateTime());
        Assert.AreEqual(6.9m, dividend.GetProperty("total").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetWalletDividendsEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        var response = await Client
            .GetAsync($"v1/wallets/{Guid.NewGuid()}/dividends", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
