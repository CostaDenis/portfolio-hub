using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Wallets;

[TestClass]
public class BuyAssetCommandHandlerTests
{

    private readonly Wallet _wallet;
    private readonly FakeWalletRepository _walletRepository;
    private readonly WalletFinder _walletFinder;
    private readonly Asset _asset;
    private readonly FakeAssetRepository _assetRepository;
    private readonly AssetFinder _assetFinder;
    private readonly BuyAssetCommandHandler _handler;

    public BuyAssetCommandHandlerTests()
    {
        _wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        _walletRepository = new FakeWalletRepository(_wallet);
        _walletFinder = new WalletFinder(_walletRepository);

        _asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        _assetRepository = new FakeAssetRepository(_asset);
        _assetFinder = new AssetFinder(_assetRepository);

        _handler = new BuyAssetCommandHandler(_walletRepository, _walletFinder, _assetFinder);
    }

    [TestMethod]
    [TestCategory("BuyAssetCommandHandler tests")]
    public async Task Should_Return_Exception_When_MoneyValue_Is_Negative()
    {
        var command = new BuyAssetCommand(_wallet.Id, _asset.Id, new Quantity(2), new Money(-105m));

        await Assert.ThrowsAsync<InvalidPriceException>
            (async () => await _handler.HandleAsync(command, CancellationToken.None));
        Assert.IsFalse(_walletRepository.UpdateWasCalled);
    }

    [TestMethod]
    [TestCategory("BuyAssetCommandHandler tests")]
    public async Task Should_Buy_Asset_And_Persist_Wallet()
    {
        var command = new BuyAssetCommand(_wallet.Id, _asset.Id, new Quantity(2), new Money(105m));

        await _handler.HandleAsync(command, CancellationToken.None);

        Assert.AreEqual(new Quantity(2), _wallet.GetCurrentQuantity(_asset));
        Assert.IsTrue(_walletRepository.UpdateWasCalled);
        Assert.AreSame(_wallet, _walletRepository.UpdatedWallet);
    }
}