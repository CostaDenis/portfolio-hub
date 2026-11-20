using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PortfolioHub.Api.Tests.Providers;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Api.Tests.Endpoints.Assets;

[TestClass]
public class SyncMarketPricesEndpointTests : ApiTestBase
{

    [TestMethod]
    [TestCategory("SyncMarketPricesEndpoint Tests")]
    public async Task Should_Return_Ok_And_Update_Crypto_Prices()
    {
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(300000m));   //preco diferente estabelecido no FakeMarketPriceProvider

        await using (var createContext = CreateDbContext())
        {
            await createContext.Assets.AddAsync(bitcoin, CancellationToken.None);
            await createContext.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync("v1/assets/prices/sync?force=true", null, CancellationToken.None);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var repository = new AssetRepository(verificationContext);
        var storedBtc = await repository.GetByIdAsync(bitcoin.Id, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(1, body.GetProperty("updated").GetInt32());
        Assert.AreEqual(0, body.GetProperty("notFound").GetInt32());
        Assert.IsNotNull(storedBtc);
        Assert.AreEqual(FakeMarketPriceProvider.BtcPrice, (decimal)storedBtc.MarketPrice.Price);
    }

    [TestMethod]
    [TestCategory("SyncMarketPricesEndpoint Tests")]
    public async Task Should_Skip_Recently_Updated_Assets_When_Not_Forced()
    {
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(300000m));

        await using (var setupContext = CreateDbContext())
        {
            await setupContext.Assets.AddAsync(bitcoin, CancellationToken.None);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync("v1/assets/prices/sync", null, CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(0, body.GetProperty("updated").GetInt32());
        Assert.AreEqual(1, body.GetProperty("skipped").GetInt32());
    }

    [TestMethod]
    [TestCategory("SyncMarketPricesEndpoint Tests")]
    public async Task Should_Count_Assets_Without_Provider_As_Not_Found()
    {
        var bbas3 = new Asset(new AssetName("Banco do Brasil Sa"), new Ticker("BBAS3"),
            EAssetType.Stock, new MarketPrice(16m));

        await using (var setupContext = CreateDbContext())
        {
            await setupContext.Assets.AddAsync(bbas3, CancellationToken.None);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync("v1/assets/prices/sync", null, CancellationToken.None);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var repository = new AssetRepository(verificationContext);

        var storedBbas3 = await repository.GetByIdAsync(bbas3.Id, CancellationToken.None);

        Assert.IsNotNull(storedBbas3);
        Assert.AreEqual(new MarketPrice(16m), storedBbas3.MarketPrice);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(0, body.GetProperty("updated").GetInt32());
        Assert.AreEqual(1, body.GetProperty("notFound").GetInt32());
    }
}