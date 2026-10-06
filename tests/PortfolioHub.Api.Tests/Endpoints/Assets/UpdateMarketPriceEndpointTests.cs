using System.Net;
using System.Net.Http.Json;
using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class UpdateMarketPriceEndpointTests : ApiTestBase
{

    private readonly Asset _xpml11;

    public UpdateMarketPriceEndpointTests()
    {
        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));
    }

    [TestMethod]
    [TestCategory("UpdateMarketPriceEndpoint tests")]
    public async Task Should_Return_NoContent_When_MarketPrice_Is_Valid()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new UpdateMarketPriceRequest(110m);
        var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}/market-price", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UpdateMarketPriceEndpoint tests")]
    public async Task Should_Return_BadRequest_When_MarketPrice_Is_Invalid()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        //Preço negativo
        var request = new UpdateMarketPriceRequest(-110m);
        var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}/market-price", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UpdateMarketPriceEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        var request = new UpdateMarketPriceRequest(10m);
        var response = await Client
            .PutAsJsonAsync($"v1/assets/{Guid.NewGuid()}/market-price", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
