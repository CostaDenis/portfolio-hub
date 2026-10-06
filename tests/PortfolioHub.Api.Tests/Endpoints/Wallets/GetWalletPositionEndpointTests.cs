using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletPositionEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("GetWalletPositionEndpoint tests")]
    public async Task Should_Return_Ok_With_WalletPosition_Data()
    {
        var wallet = new Wallet(new WalletName("Criptomoedas"));
        var bitcoin = new Asset(new AssetName("Bitcoin"),
            new Ticker("BTC"), EAssetType.Cryptocurrency, new MarketPrice(400000m));
        var wibx = new Asset(new AssetName("Wibx"),
            new Ticker("WBX"), EAssetType.Cryptocurrency, new MarketPrice(0.04m));
        var bitcoinQuantity = 0.002m;
        var wibxQuantity = 10000m;

        wallet.BuyAsset(bitcoin, bitcoinQuantity, new Money(380000m));
        wallet.BuyAsset(wibx, wibxQuantity, new Money(0.03m));

        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(bitcoin, CancellationToken.None);
            await context.Assets.AddAsync(wibx, CancellationToken.None);
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}/position", CancellationToken.None);
        using var json = JsonDocument
            .Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;
        var bitcoinJson = body.EnumerateArray()
            .Single(item => item.GetProperty("assetId").GetGuid() == bitcoin.Id);
        var wibxJson = body.EnumerateArray()
            .Single(item => item.GetProperty("assetId").GetGuid() == wibx.Id);


        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(JsonValueKind.Array, body.ValueKind);
        Assert.AreEqual(2, body.GetArrayLength());

        Assert.AreEqual("Bitcoin", bitcoinJson.GetProperty("assetName").GetString());
        Assert.AreEqual("BTC", bitcoinJson.GetProperty("ticker").GetString());
        Assert.AreEqual(bitcoinQuantity, bitcoinJson.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(400000m, bitcoinJson.GetProperty("marketPrice").GetDecimal());

        Assert.AreEqual("Wibx", wibxJson.GetProperty("assetName").GetString());
        Assert.AreEqual("WBX", wibxJson.GetProperty("ticker").GetString());
        Assert.AreEqual(wibxQuantity, wibxJson.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(0.04m, wibxJson.GetProperty("marketPrice").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetWalletPositionEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        using var response = await Client
           .GetAsync($"v1/wallets/{Guid.NewGuid()}/position", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
