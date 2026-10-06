using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class CreateAssetEndpointTests : ApiTestBase
{

    private Asset _asset;
    private readonly CreateAssetRequest _request;

    public CreateAssetEndpointTests()
    {
        _asset = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(400000m));

        _request = new CreateAssetRequest("Bitcoin", "BTC",
            EAssetType.Cryptocurrency, 400000m);
    }

    [TestMethod]
    public async Task Should_Return_Created_When_Request_Is_Valid()
    {
        var response = await Client
            .PostAsJsonAsync($"v1/assets", _request, CancellationToken.None);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var body = json.RootElement;
        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets.FirstOrDefaultAsync(x => x.Name == "Bitcoin");

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(storedAsset);
        Assert.AreEqual("Bitcoin", body.GetProperty("name").GetString());
        Assert.AreEqual("BTC", body.GetProperty("ticker").GetString());
        Assert.AreEqual("Cryptocurrency", body.GetProperty("type").GetString());
        Assert.AreEqual(400000m, body.GetProperty("price").GetDecimal());
    }

    [TestMethod]
    [TestCategory("CreateAssetEndpoint tests")]
    [DataRow("", "BTC", 400000)]
    [DataRow("Bitcoin", "", 400000)]
    [DataRow("Bitcoin", "BTC", -1)]
    public async Task Should_Return_BadRequest_Without_Creating_Asset_When_Request_Is_Invalid(
        string assetName, string ticker, int marketPrice)
    {
        var request = new CreateAssetRequest(assetName, ticker,
            EAssetType.Cryptocurrency, marketPrice);
        using var response = await Client
            .PostAsJsonAsync("v1/assets", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.AreEqual(0, await verificationContext.Assets.CountAsync());
    }

    [TestMethod]
    [TestCategory("CreateAssetEndpoint tests")]
    public async Task Should_Return_Conflict_Without_Creating_Asset_When_Ticker_Already_Exists()
    {
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_asset, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new CreateAssetRequest("Outro ativo", _request.Ticker,
            EAssetType.Cryptocurrency, 500000m);
        using var response = await Client
            .PostAsJsonAsync("v1/assets", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets.SingleAsync();

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);

        Assert.AreEqual(1, await verificationContext.Assets.CountAsync());
        Assert.AreEqual(_asset.Id, storedAsset.Id);
        Assert.AreEqual("Bitcoin", storedAsset.Name.Value);
        Assert.AreEqual("BTC", storedAsset.Ticker.Value);
        Assert.AreEqual(EAssetType.Cryptocurrency, storedAsset.Type);
        Assert.AreEqual(400000m, storedAsset.MarketPrice.Price.Value);
    }
}
