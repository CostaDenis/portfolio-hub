using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Infrastructure.Tests.Repositories;

[TestClass]
public class AssetRepositoryTests : InfrastructureTestBase
{
    private readonly Asset _xpml11;
    private readonly Asset _btc;

    public AssetRepositoryTests()
    {
        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));
        _btc = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(new Money(350000m)));
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_Asset_With_ValueObjects_By_Id()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(_xpml11);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(_xpml11.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(_xpml11.Id, result.Id);
        Assert.AreEqual("XP Malls", result.Name.Value);
        Assert.AreEqual("XPML11", result.Ticker.Value);
        Assert.AreEqual(EAssetType.RealStateFund, result.Type);
        Assert.AreEqual(105.12345678m, result.MarketPrice.Price.Value);
        Assert.AreEqual(_xpml11.MarketPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_Null_When_Asset_Does_Not_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_All_Assets_Without_Tracking()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.AddRange(_xpml11, _btc);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(2, result);
        CollectionAssert.AreEquivalent(
            new[] { _xpml11.Id, _btc.Id }, result.Select(asset => asset.Id).ToArray());
        Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_Empty_List_When_No_Assets_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(0, result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Persist_Updated_Asset_And_MarketPrice()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(_xpml11);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        var updatedPrice = new MarketPrice(new Money(120.87654321m));

        await using (var updateContext = CreateDbContext())
        {
            var repository = new AssetRepository(updateContext);
            var storedAsset = await repository.GetByIdAsync(_xpml11.Id, CancellationToken.None);
            Assert.IsNotNull(storedAsset);

            storedAsset.UpdateName(new AssetName("Ativo atualizado"));
            storedAsset.UpdateTicker(new Ticker("NOVO3"));
            storedAsset.UpdateType(EAssetType.Stock);
            storedAsset.UpdateMarketPrice(updatedPrice);

            await repository.UpdateAsync(storedAsset, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new AssetRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_xpml11.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Ativo atualizado", result.Name.Value);
        Assert.AreEqual("NOVO3", result.Ticker.Value);
        Assert.AreEqual(EAssetType.Stock, result.Type);
        Assert.AreEqual(120.87654321m, result.MarketPrice.Price.Value);
        Assert.AreEqual(updatedPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_True_When_Ticker_Exists_And_No_Id_Is_Excluded()
    {
        await SeedAssetAsync();

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.ExistsByTickerAsync(
            new Ticker("xpml11"), null, CancellationToken.None);

        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_False_When_Ticker_Does_Not_Exist()
    {
        await SeedAssetAsync();

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.ExistsByTickerAsync(
            new Ticker("BTC"), null, CancellationToken.None);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_False_When_Ticker_Belongs_Only_To_Excluded_Asset()
    {
        await SeedAssetAsync();

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.ExistsByTickerAsync(
            new Ticker("XPML11"), _xpml11.Id, CancellationToken.None);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Return_True_When_Ticker_Belongs_To_Another_Asset()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.AddRange(_xpml11, _btc);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.ExistsByTickerAsync(
            new Ticker("XPML11"), _btc.Id, CancellationToken.None);

        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository Tests")]
    public async Task Should_Persist_Updated_Prices_When_Updating_Range()
    {
        var originalLastUpdateXpml11 = _xpml11.MarketPrice.LastUpdate;
        var originalLastUpdateBTC = _btc.MarketPrice.LastUpdate;

        await using (var setupContext = CreateDbContext())
        {
            await setupContext.Assets.AddAsync(_btc, CancellationToken.None);
            await setupContext.Assets.AddAsync(_xpml11, CancellationToken.None);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using (var updateContext = CreateDbContext())
        {
            var assetRepository = new AssetRepository(updateContext);
            var assets = await assetRepository.GetAllAssets(CancellationToken.None);

            foreach (var asset in assets)
                asset.UpdateMarketPrice(new MarketPrice(1m));

            await assetRepository.UpdateRangeAsync(assets, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var repository = new AssetRepository(verificationContext);

        var storedBtc = await repository.GetByIdAsync(_btc.Id, CancellationToken.None);
        var storedXpml11 = await repository.GetByIdAsync(_xpml11.Id, CancellationToken.None);

        Assert.IsNotNull(storedBtc);
        Assert.IsNotNull(storedXpml11);

        Assert.AreEqual(new MarketPrice(1m), storedBtc.MarketPrice);
        Assert.AreEqual(new MarketPrice(1m), storedXpml11.MarketPrice);

        Assert.IsTrue(storedBtc.MarketPrice.LastUpdate > originalLastUpdateBTC);
        Assert.IsTrue(storedXpml11.MarketPrice.LastUpdate > originalLastUpdateXpml11);
    }

    private async Task SeedAssetAsync()
    {
        await using var setupContext = CreateDbContext();
        setupContext.Assets.Add(_xpml11);
        await setupContext.SaveChangesAsync(CancellationToken.None);
    }
}
