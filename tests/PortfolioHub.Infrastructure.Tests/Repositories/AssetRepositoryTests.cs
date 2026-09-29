using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Infrastructure.Tests.Repositories;

[TestClass]
public class AssetRepositoryTests : InfrastructureTestBase
{
    private readonly Asset _asset;

    public AssetRepositoryTests()
    {
        _asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));
    }

    [TestMethod]
    [TestCategory("AssetRepository tests")]
    public async Task Should_Return_Asset_With_ValueObjects_By_Id()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(_asset);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(_asset.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(_asset.Id, result.Id);
        Assert.AreEqual("XP Malls", result.Name.Value);
        Assert.AreEqual("XPML11", result.Ticker.Value);
        Assert.AreEqual(EAssetType.RealStateFund, result.Type);
        Assert.AreEqual(105.12345678m, result.MarketPrice.Price.Value);
        Assert.AreEqual(_asset.MarketPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }

    [TestMethod]
    [TestCategory("AssetRepository tests")]
    public async Task Should_Return_Null_When_Asset_Does_Not_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("AssetRepository tests")]
    public async Task Should_Return_All_Assets_Without_Tracking()
    {
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(new Money(350000m)));

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.AddRange(_asset, bitcoin);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(2, result);
        CollectionAssert.AreEquivalent(
            new[] { _asset.Id, bitcoin.Id }, result.Select(asset => asset.Id).ToArray());
        Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    [TestCategory("AssetRepository tests")]
    public async Task Should_Return_Empty_List_When_No_Assets_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(0, result);
    }

    [TestMethod]
    [TestCategory("AssetRepository tests")]
    public async Task Should_Persist_Updated_Asset_And_MarketPrice()
    {
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(_asset);
            await setupContext.SaveChangesAsync(CancellationToken.None);
        }

        var updatedPrice = new MarketPrice(new Money(120.87654321m));

        await using (var updateContext = CreateDbContext())
        {
            var repository = new AssetRepository(updateContext);
            var storedAsset = await repository.GetByIdAsync(_asset.Id, CancellationToken.None);
            Assert.IsNotNull(storedAsset);

            storedAsset.UpdateName(new AssetName("Ativo atualizado"));
            storedAsset.UpdateTicker(new Ticker("NOVO3"));
            storedAsset.UpdateType(EAssetType.Stock);
            storedAsset.UpdateMarketPrice(updatedPrice);

            await repository.UpdateAsync(storedAsset, CancellationToken.None);
        }

        await using var verificationContext = CreateDbContext();
        var verificationRepository = new AssetRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(_asset.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Ativo atualizado", result.Name.Value);
        Assert.AreEqual("NOVO3", result.Ticker.Value);
        Assert.AreEqual(EAssetType.Stock, result.Type);
        Assert.AreEqual(120.87654321m, result.MarketPrice.Price.Value);
        Assert.AreEqual(updatedPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }
}
