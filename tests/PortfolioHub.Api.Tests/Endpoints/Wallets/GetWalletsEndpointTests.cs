using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletsEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("GetWalletsEndpoint tests")]
    public async Task Should_Return_Ok_With_Wallet_Summaries()
    {
        var bitcoin = new Asset(new AssetName("Bitcoin"),
            new Ticker("BTC"), EAssetType.Cryptocurrency, new MarketPrice(400000m));
        var crypto = new Wallet(new WalletName("Criptomoedas"));
        var emptyWallet = new Wallet(new WalletName("Reserva"));

        crypto.BuyAsset(bitcoin, 0.002m, new Money(380000m));

        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(bitcoin, CancellationToken.None);
            await context.Wallets.AddRangeAsync([crypto, emptyWallet], CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.GetAsync("v1/wallets", CancellationToken.None);
        using var json = JsonDocument
            .Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;
        var cryptoJson = body.EnumerateArray()
            .Single(item => item.GetProperty("walletId").GetGuid() == crypto.Id);
        var emptyJson = body.EnumerateArray()
            .Single(item => item.GetProperty("walletId").GetGuid() == emptyWallet.Id);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(2, body.GetArrayLength());

        Assert.AreEqual("Criptomoedas", cryptoJson.GetProperty("name").GetString());
        Assert.AreEqual(0.002m * 400000m, cryptoJson.GetProperty("netWorth").GetDecimal());
        Assert.AreEqual(1, cryptoJson.GetProperty("assetCount").GetInt32());

        Assert.AreEqual("Reserva", emptyJson.GetProperty("name").GetString());
        Assert.AreEqual(0m, emptyJson.GetProperty("netWorth").GetDecimal());
        Assert.AreEqual(0, emptyJson.GetProperty("assetCount").GetInt32());
    }

    [TestMethod]
    [TestCategory("GetWalletsEndpoint tests")]
    public async Task Should_Return_Ok_With_Empty_List_When_No_Wallets_Exist()
    {
        using var response = await Client.GetAsync("v1/wallets", CancellationToken.None);
        using var json = JsonDocument
            .Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(0, json.RootElement.GetArrayLength());
    }
}
