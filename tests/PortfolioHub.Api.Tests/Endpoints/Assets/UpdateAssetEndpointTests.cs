using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class UpdateAssetEndpointTests : ApiTestBase
{

    private readonly Asset _xpml11;
    private readonly Asset _btci11;

    public UpdateAssetEndpointTests()
    {
        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));

        _btci11 = new Asset(new AssetName("BTG"), new Ticker("BTCI11"),
          EAssetType.RealStateFund, new MarketPrice(new Money(9m)));
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Update_Asset_And_Return_NoContent()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new UpdateAssetRequest("XP Atualizado", "XPNEW", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.AreEqual(string.Empty, await response.Content.ReadAsStringAsync());

        await using var updateContext = CreateDbContext();
        var storedAsset = await updateContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);

        Assert.IsNotNull(storedAsset);
        Assert.AreEqual(new AssetName("XP Atualizado"), storedAsset.Name);
        Assert.AreEqual(new Ticker("XPNEW"), storedAsset.Ticker);
        Assert.AreEqual(EAssetType.Stock, storedAsset.Type);
        Assert.AreEqual(105.12345678m, storedAsset.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        var request = new UpdateAssetRequest("XP Atualizado", "XPNEW", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{Guid.NewGuid()}", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_BadRequest_Without_Changing_Asset_When_Name_Is_Greater_Than_60_Characters()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        //AssetName permite até 60 caracteres
        var request = new UpdateAssetRequest
            ("XPMallsXPMallsXPMallsXPMallsXPMallsXPMallsXPMallsXPMallsXPMallsXPMalls",
                "XPM", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedAsset);
        Assert.AreEqual(new AssetName("XP Malls"), storedAsset.Name);
        Assert.AreEqual(new Ticker("XPML11"), storedAsset.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedAsset.Type);
        Assert.AreEqual(105.12345678m, storedAsset.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_BadRequest_Without_Changing_Asset_When_Name_Is_Smaller_Than_3_Characters()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        //AssetName permite pelo menos 3 caracteres
        var request = new UpdateAssetRequest
            ("XP", "XPM", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedAsset);
        Assert.AreEqual(new AssetName("XP Malls"), storedAsset.Name);
        Assert.AreEqual(new Ticker("XPML11"), storedAsset.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedAsset.Type);
        Assert.AreEqual(105.12345678m, storedAsset.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_BadRequest_Without_Changing_Asset_When_Ticker_Is_Greater_Than_10_Characters()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        //Ticker permite até 10 caracteres
        var request = new UpdateAssetRequest
            ("XP Novo", "XPML11XPML11", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedAsset);
        Assert.AreEqual(new AssetName("XP Malls"), storedAsset.Name);
        Assert.AreEqual(new Ticker("XPML11"), storedAsset.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedAsset.Type);
        Assert.AreEqual(105.12345678m, storedAsset.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_BadRequest_Without_Changing_Asset_When_Ticker_Is_Smaller_Than_2_Characters()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        //Ticker permite pelo menos 2 caracteres
        var request = new UpdateAssetRequest
            ("XP Novo", "X", EAssetType.Stock);

        using var response = await Client
            .PutAsJsonAsync($"v1/assets/{_xpml11.Id}", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedAsset = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedAsset);
        Assert.AreEqual(new AssetName("XP Malls"), storedAsset.Name);
        Assert.AreEqual(new Ticker("XPML11"), storedAsset.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedAsset.Type);
        Assert.AreEqual(105.12345678m, storedAsset.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("UpdateAssetEndpoint tests")]
    public async Task Should_Return_Conflict_Without_Changing_Assets_When_Ticker_Belongs_To_Another_Asset()
    {
        await using (var context = CreateDbContext())
        {
            context.Assets.Add(_xpml11);
            context.Assets.Add(_btci11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        //Ticker já cadastrado
        var request = new UpdateAssetRequest("BTG Novo", "XPML11", EAssetType.Stock);

        var response = await Client
            .PutAsJsonAsync($"v1/assets/{_btci11.Id}", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedXPML11 = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _xpml11.Id);
        var storedBTCI11 = await verificationContext.Assets
            .FirstOrDefaultAsync(x => x.Id == _btci11.Id);

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);

        Assert.IsNotNull(storedXPML11);
        Assert.AreEqual(new AssetName("XP Malls"), storedXPML11.Name);
        Assert.AreEqual(new Ticker("XPML11"), storedXPML11.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedXPML11.Type);
        Assert.AreEqual(105.12345678m, storedXPML11.MarketPrice.Price.Value);

        Assert.IsNotNull(storedBTCI11);
        Assert.AreEqual(new AssetName("BTG"), storedBTCI11.Name);
        Assert.AreEqual(new Ticker("BTCI11"), storedBTCI11.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund, storedBTCI11.Type);
        Assert.AreEqual(9m, storedBTCI11.MarketPrice.Price.Value);
    }

}
