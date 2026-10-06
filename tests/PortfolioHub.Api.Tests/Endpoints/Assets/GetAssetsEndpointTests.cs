using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class GetAssetsEndpointTests : ApiTestBase
{

    [TestMethod]
    [TestCategory("GetAssetsEndpoint tests")]
    public async Task Should_Return_Ok_With_Assets_When_Exists()
    {
        var xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));

        var btci11 = new Asset(new AssetName("BTG Pactual Credito"), new Ticker("BTCI11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(9M)));

        await using (var context = CreateDbContext())
        {
            context.Assets.Add(xpml11);
            context.Assets.Add(btci11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync("v1/assets", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse
            (await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;

        Assert.AreEqual(JsonValueKind.Array, body.ValueKind);
        Assert.AreEqual(2, body.GetArrayLength());

        var xpml11Json = body.EnumerateArray()
            .Single(item => item.GetProperty("assetId").GetGuid() == xpml11.Id);
        var btci11Json = body.EnumerateArray()
            .Single(item => item.GetProperty("assetId").GetGuid() == btci11.Id);

        Assert.AreEqual("XP Malls", xpml11Json.GetProperty("name").GetString());
        Assert.AreEqual("XPML11", xpml11Json.GetProperty("ticker").GetString());
        Assert.AreEqual("RealStateFund", xpml11Json.GetProperty("type").GetString());
        Assert.AreEqual(105.12345678m, xpml11Json.GetProperty("price").GetDecimal());

        Assert.AreEqual("BTG Pactual Credito", btci11Json.GetProperty("name").GetString());
        Assert.AreEqual("BTCI11", btci11Json.GetProperty("ticker").GetString());
        Assert.AreEqual("RealStateFund", btci11Json.GetProperty("type").GetString());
        Assert.AreEqual(9m, btci11Json.GetProperty("price").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetAssetsEndpoint tests")]
    public async Task Should_Return_Ok_With_Empty_Array_When_No_Assets_Exist()
    {
        using var response = await Client
            .GetAsync("v1/assets", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse
            (await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;

        //Mesmo com OK, deve retornar um array vazio
        //quando não houver Assets cadastrados
        Assert.AreEqual(JsonValueKind.Array, body.ValueKind);
        Assert.AreEqual(0, body.GetArrayLength());
    }
}
