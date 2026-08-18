using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Services;

[TestClass]
public class AssetFinderTests
{

    private readonly Asset _asset;
    private readonly FakeAssetRepository _repository;
    private readonly AssetFinder _finder;

    public AssetFinderTests()
    {
        _asset = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(330000m));

        _repository = new FakeAssetRepository(_asset);
        _finder = new AssetFinder(_repository);
    }

    [TestMethod]
    [TestCategory("AssetFinder tests")]
    public void Should_Return_Exception_When_NotFound_Asset()
        => Assert.ThrowsAsync<AssetNotFoundException>
            (async () => await _finder.GetRequiredAsync(Guid.NewGuid(), CancellationToken.None));

    [TestMethod]
    [TestCategory("AssetFinder tests")]
    public async Task Should_Return_Asset()
    {
        var foundAsset = await _finder.GetRequiredAsync(_asset.Id, CancellationToken.None);

        Assert.AreSame(foundAsset, _asset);
    }
}