using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class GetAssetByIdEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("GetAssetByIdEndpoint tests")]
    public async Task Should_Return_Ok_With_Asset_When_Asset_Exists()
    {
        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));

        await using (var context = CreateDbContext())
        {
            context.Assets.Add(asset);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync($"/v1/assets/{asset.Id}", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse
            (await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;

        Assert.AreEqual(asset.Id, body.GetProperty("assetId").GetGuid());
        Assert.AreEqual("XP Malls", body.GetProperty("name").GetString());
        Assert.AreEqual("XPML11", body.GetProperty("ticker").GetString());
        Assert.AreEqual("RealStateFund", body.GetProperty("type").GetString());
        Assert.AreEqual(105.12345678m, body.GetProperty("price").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetAssetByIdEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        using var response = await Client
            .GetAsync($"/v1/assets/{Guid.NewGuid()}", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("GetAssetByIdEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Id_Is_Not_A_Guid()
    {
        using var response = await Client
            .GetAsync("/v1/assets/abc", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
