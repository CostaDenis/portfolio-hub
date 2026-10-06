using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletAssetPositionEndpointTests : ApiTestBase
{

    private readonly Wallet _wallet;
    private readonly Asset _asset;

    public GetWalletAssetPositionEndpointTests()
    {
        _wallet = new Wallet(new WalletName("Criptomoedas"));
        _asset = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(400000m));
    }

    [TestMethod]
    [TestCategory("GetWalletAssetPositionEndpoint tests")]
    public async Task Should_Return_Ok_When_Request_Is_Valid()
    {
        _wallet.BuyAsset(_asset, 0.0010m, new Money(390000m));

        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_asset, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var response = await Client
            .GetAsync($"v1/wallets/{_wallet.Id}/assets/{_asset.Id}/position", CancellationToken.None);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(_asset.Id, body.GetProperty("assetId").GetGuid());
        Assert.AreEqual("BTC", body.GetProperty("ticker").GetString());
        Assert.AreEqual("Bitcoin", body.GetProperty("assetName").GetString());
        Assert.AreEqual(0.0010m, body.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(400000m, body.GetProperty("marketPrice").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetWalletAssetPositionEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_asset, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var response = await Client
            .GetAsync($"v1/wallets/{Guid.NewGuid()}/assets/{_asset.Id}/position", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("GetWalletAssetPositionEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var response = await Client
            .GetAsync($"v1/wallets/{_wallet.Id}/assets/{Guid.NewGuid()}/position", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
