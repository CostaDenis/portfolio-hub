using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Infrastructure.Repositories;

namespace PortfolioHub.Infrastructure.Tests.Repositories;

[TestClass]
public class AssetRepositoryTests : InfrastructureTestBase
{
    [TestMethod]
    public async Task Should_Return_Asset_With_ValueObjects_By_Id()
    {
        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));

        // O repository ainda não tem Create: o contexto prepara os dados do teste.
        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(asset);
            await setupContext.SaveChangesAsync();
        }

        // Outro contexto garante que os dados sejam materializados do banco.
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(asset.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(asset.Id, result.Id);
        Assert.AreEqual("XP Malls", result.Name.Value);
        Assert.AreEqual("XPML11", result.Ticker.Value);
        Assert.AreEqual(EAssetType.RealStateFund, result.Type);
        Assert.AreEqual(105.12345678m, result.MarketPrice.Price.Value);
        Assert.AreEqual(asset.MarketPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }

    [TestMethod]
    public async Task Should_Return_Null_When_Asset_Does_Not_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Should_Return_All_Assets_Without_Tracking()
    {
        var xpml = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105m)));
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(new Money(350000m)));

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.AddRange(xpml, bitcoin);
            await setupContext.SaveChangesAsync();
        }

        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(2, result);
        CollectionAssert.AreEquivalent(
            new[] { xpml.Id, bitcoin.Id }, result.Select(asset => asset.Id).ToArray());
        Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    public async Task Should_Return_Empty_List_When_No_Assets_Exist()
    {
        await using var context = CreateDbContext();
        var repository = new AssetRepository(context);

        var result = await repository.GetAllAssets(CancellationToken.None);

        Assert.HasCount(0, result);
    }

    [TestMethod]
    public async Task Should_Persist_Updated_Asset_And_MarketPrice()
    {
        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105m)));

        await using (var setupContext = CreateDbContext())
        {
            setupContext.Assets.Add(asset);
            await setupContext.SaveChangesAsync();
        }

        var updatedPrice = new MarketPrice(new Money(120.87654321m));

        await using (var updateContext = CreateDbContext())
        {
            var repository = new AssetRepository(updateContext);
            var storedAsset = await repository.GetByIdAsync(asset.Id, CancellationToken.None);
            Assert.IsNotNull(storedAsset);

            storedAsset.UpdateName(new AssetName("Ativo atualizado"));
            storedAsset.UpdateTicker(new Ticker("NOVO3"));
            storedAsset.UpdateType(EAssetType.Stock);
            storedAsset.UpdateMarketPrice(updatedPrice);

            await repository.UpdateAsync(storedAsset, CancellationToken.None);
        }

        // A verificação usa um terceiro contexto para comprovar a persistência.
        await using var verificationContext = CreateDbContext();
        var verificationRepository = new AssetRepository(verificationContext);
        var result = await verificationRepository.GetByIdAsync(asset.Id, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("Ativo atualizado", result.Name.Value);
        Assert.AreEqual("NOVO3", result.Ticker.Value);
        Assert.AreEqual(EAssetType.Stock, result.Type);
        Assert.AreEqual(120.87654321m, result.MarketPrice.Price.Value);
        Assert.AreEqual(updatedPrice.LastUpdate, result.MarketPrice.LastUpdate);
    }
}
