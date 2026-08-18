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
public class ReceiveDividendCommandHandlerTests
{

    private readonly Wallet _wallet;
    private readonly FakeWalletRepository _walletRepository;
    private readonly WalletFinder _walletFinder;
    private readonly Asset _asset;
    private readonly FakeAssetRepository _assetRepository;
    private readonly AssetFinder _assetFinder;
    private readonly ReceiveDividendCommandHandler _handler;

    public ReceiveDividendCommandHandlerTests()
    {
        _wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        _walletRepository = new FakeWalletRepository(_wallet);
        _walletFinder = new WalletFinder(_walletRepository);

        _asset = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(110m));
        _assetRepository = new FakeAssetRepository(_asset);
        _assetFinder = new AssetFinder(_assetRepository);

        _handler = new ReceiveDividendCommandHandler(_walletRepository, _walletFinder, _assetFinder);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendCommandHandler tests")]
    public async Task Should_Return_Exception_When_Wallet_Has_Insufficient_Position_To_Receive_Dividend()
    {
        var command = new ReceiveDividendCommand(_wallet.Id, _asset.Id, new Money(0.92m), DateTime.UtcNow);

        await Assert.ThrowsAsync<InsufficientAssetQuantityException>
            (() => _handler.HandleAsync(command, CancellationToken.None));
        Assert.IsFalse(_walletRepository.UpdateWasCalled);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendCommandHandler tests")]
    public async Task Should_Receive_Dividend_And_Persist_Wallet()
    {
        _wallet.BuyAsset(_asset, new Quantity(5), new Money(100m));

        var command = new ReceiveDividendCommand(_wallet.Id, _asset.Id, new Money(0.92m), DateTime.UtcNow);

        await _handler.HandleAsync(command, CancellationToken.None);

        var dividend = _wallet.Dividends.Single();
        var correctDividendTotal = 0.92m * new Quantity(5);

        Assert.HasCount(1, _wallet.Dividends);
        Assert.AreSame(_asset, dividend.Asset);
        Assert.AreEqual(new Money(0.92m), dividend.ValuePerShare);
        Assert.AreEqual(new Quantity(5), dividend.Quantity);
        Assert.AreEqual(correctDividendTotal, dividend.Total.Value);
        Assert.IsTrue(_walletRepository.UpdateWasCalled);
        Assert.AreEqual(new Quantity(5), _wallet.GetCurrentQuantity(_asset));
        Assert.AreSame(_wallet, _walletRepository.UpdatedWallet);
    }
}