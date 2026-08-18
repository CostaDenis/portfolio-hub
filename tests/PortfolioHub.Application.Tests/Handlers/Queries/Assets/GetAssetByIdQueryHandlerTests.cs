using PortfolioHub.Application.Handlers.Queries.Assets;
using PortfolioHub.Application.Queries.Assets;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Assets;

[TestClass]
public class GetAssetByIdQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetAssetByIdQueryHandler tests")]
    public async Task Should_Return_AssetDTO_Without_Changing_Asset()
    {
        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var repository = new FakeAssetRepository(asset);
        var handler = new GetAssetByIdQueryHandler(repository);
        var query = new GetAssetByIdQuery(asset.Id);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(asset.Id, result.AssetId);
        Assert.AreEqual("XP Malls", result.Name);
        Assert.AreEqual("XPML11", result.Ticker);
        Assert.AreEqual(EAssetType.RealStateFund.ToString(), result.Type);
        Assert.AreEqual(110m, result.Price);
        Assert.IsFalse(repository.UpdateWasCalled);
    }
}
