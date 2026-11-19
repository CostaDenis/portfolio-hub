using Microsoft.Extensions.Logging.Abstractions;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Application.Tests.Providers;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Assets;

[TestClass]
public class SyncMarketPricesCommandHandlerTests
{
    private readonly Asset _btc = new(new AssetName("Bitcoin"), new Ticker("BTC"),
        EAssetType.Cryptocurrency, new MarketPrice(330000m));

    private readonly Asset _eth = new(new AssetName("Ethereum"), new Ticker("ETH"),
        EAssetType.Cryptocurrency, new MarketPrice(9000m));

    private readonly Asset _bbas3 = new(new AssetName("Banco do brasil SA"), new Ticker("BBAS3"),
        EAssetType.Stock, new MarketPrice(16m));

    [TestMethod]
    [TestCategory("SyncMarketPricesCommandHandler Tests")]
    public async Task Should_Update_Prices_Returned_By_Provider()
    {
        var repository = new FakeAssetRepository([_btc, _eth]);
        var provider = new FakeMarketPriceProvider(EAssetType.Cryptocurrency,
            prices: new Dictionary<string, decimal>
            {
                ["BTC"] = 367000m,
                ["ETH"] = 10280m,
            });
        var handler = new SyncMarketPricesCommandHandler(repository, [provider], NullLogger<SyncMarketPricesCommandHandler>.Instance);

        var result = await handler.HandleAsync(new SyncMarketPricesCommand(), CancellationToken.None);

        Assert.AreEqual(2, result.Updated);
        Assert.AreEqual(0, result.NotFound);
        Assert.IsTrue(repository.UpdateRangeWasCalled);
        Assert.HasCount(2, repository.UpdatedAssets);
        Assert.AreEqual(367000m, _btc.MarketPrice.Price.Value);
        Assert.AreEqual(10280m, _eth.MarketPrice.Price.Value);
    }

    [TestMethod]
    [TestCategory("SyncMarketPricesCommandHandler Tests")]
    public async Task Should_Skip_Assets_Without_Provider()
    {
        var repository = new FakeAssetRepository([_bbas3, _btc]);
        var provider = new FakeMarketPriceProvider(EAssetType.Cryptocurrency,
            prices: new Dictionary<string, decimal>
            {
                ["BTC"] = 367000m,
                ["BBAS3"] = 99m //fake
            });
        var handler = new SyncMarketPricesCommandHandler(repository, [provider], NullLogger<SyncMarketPricesCommandHandler>.Instance);

        var result = await handler.HandleAsync(new SyncMarketPricesCommand(), CancellationToken.None);

        Assert.AreEqual(1, result.Updated);
        Assert.AreEqual(1, result.NotFound);
        Assert.AreEqual(new MarketPrice(16m), _bbas3.MarketPrice);
    }

    [TestMethod]
    [TestCategory("SyncMarketPricesCommandHandler Tests")]
    public async Task Should_Skip_Assets_Not_Found_By_Provider()
    {
        var repository = new FakeAssetRepository([_btc, _eth]);
        var provider = new FakeMarketPriceProvider(EAssetType.Cryptocurrency,
        prices: new Dictionary<string, decimal>
        {
            ["BTC"] = 367000m,
            //API nao conhece o ETH
        });
        var handler = new SyncMarketPricesCommandHandler(repository, [provider], NullLogger<SyncMarketPricesCommandHandler>.Instance);

        var result = await handler.HandleAsync(new SyncMarketPricesCommand(), CancellationToken.None);

        Assert.AreEqual(1, result.Updated);
        Assert.AreEqual(1, result.NotFound);
        Assert.AreEqual(new MarketPrice(367000), _btc.MarketPrice);
        Assert.AreEqual(new MarketPrice(9000m), _eth.MarketPrice);
    }

    [TestMethod]
    [TestCategory("SyncMarketPricesCommandHandler Tests")]
    public async Task Should_Not_Fail_When_Provider_Throws()
    {
        var repository = new FakeAssetRepository([_btc, _eth]);
        var provider = new FakeMarketPriceProvider(EAssetType.Cryptocurrency, shouldFail: true);
        var handler = new SyncMarketPricesCommandHandler(repository, [provider], NullLogger<SyncMarketPricesCommandHandler>.Instance);

        var result = await handler.HandleAsync(new SyncMarketPricesCommand(), CancellationToken.None);

        Assert.AreEqual(0, result.Updated);
        Assert.AreEqual(2, result.NotFound);
        Assert.IsFalse(repository.UpdateRangeWasCalled);
    }
}
