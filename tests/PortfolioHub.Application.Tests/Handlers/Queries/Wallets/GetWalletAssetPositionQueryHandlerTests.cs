using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletAssetPositionQueryHandlerTests
{
    [TestMethod]
    public async Task Should_Return_DTO_Without_Changing_Wallet()
    {
        var wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        var walletRepository = new FakeWalletRepository(wallet);
        var walletFinder = new WalletFinder(walletRepository);

        var asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        var assetRepository = new FakeAssetRepository(asset);
        var assetFinder = new AssetFinder(assetRepository);

        var handler = new GetWalletAssetPositionQueryHandler(walletFinder, assetFinder);
        var query = new GetWalletAssetPositionQuery(wallet.Id, asset.Id);

        wallet.BuyAsset(asset, new Quantity(2), new Money(105m));

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.AreEqual(asset.Id, result.AssetId);
        Assert.AreEqual(asset.Ticker.Value, result.Ticker);
        Assert.AreEqual(asset.Name.Value, result.AssetName);
        Assert.AreEqual(asset.MarketPrice.Price.Value, result.MarketPrice); Assert.IsFalse(walletRepository.UpdateWasCalled);
        Assert.IsFalse(walletRepository.CreateWasCalled);

    }
}