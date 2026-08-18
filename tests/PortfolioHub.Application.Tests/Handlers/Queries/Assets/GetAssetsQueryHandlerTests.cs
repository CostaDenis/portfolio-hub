using PortfolioHub.Application.Handlers.Queries.Assets;
using PortfolioHub.Application.Queries.Assets;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Assets;

[TestClass]
public class GetAssetsQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetAssetsQueryHandler tests")]
    public async Task Should_Return_AssetDTOs_Without_Changing_Assets()
    {
        var xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var bitcoin = new Asset(new AssetName("Bitcoin"), new Ticker("BTC"),
            EAssetType.Cryptocurrency, new MarketPrice(650_000m));
        var repository = new FakeAssetRepository([xpml11, bitcoin]);
        var handler = new GetAssetsQueryHandler(repository);

        var result = await handler.HandleAsync(new GetAssetsQuery(), CancellationToken.None);
        var xpml11Dto = result.Single(asset => asset.AssetId == xpml11.Id);
        var bitcoinDto = result.Single(asset => asset.AssetId == bitcoin.Id);

        Assert.HasCount(2, result);
        Assert.AreEqual("XPML11", xpml11Dto.Ticker);
        Assert.AreEqual(110m, xpml11Dto.Price);
        Assert.AreEqual("BTC", bitcoinDto.Ticker);
        Assert.AreEqual(EAssetType.Cryptocurrency.ToString(), bitcoinDto.Type);
        Assert.AreEqual(650_000m, bitcoinDto.Price);
        Assert.IsFalse(repository.UpdateWasCalled);
    }
}
