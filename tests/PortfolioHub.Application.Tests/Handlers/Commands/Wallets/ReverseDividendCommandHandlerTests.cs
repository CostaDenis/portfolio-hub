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
public class ReverseDividendCommandHandlerTests
{

    private readonly Wallet _wallet;
    private readonly Asset _bbas3;
    private readonly FakeWalletRepository _walletRepository;
    private readonly WalletFinder _walletFinder;

    public ReverseDividendCommandHandlerTests()
    {
        _wallet = new Wallet(new WalletName("Ações"));
        _bbas3 = new Asset(new AssetName("Banco do Brasil SA"),
            new Ticker("BBAS3"), EAssetType.Stock, new MarketPrice(17.7m));
        _walletRepository = new FakeWalletRepository(_wallet);
        _walletFinder = new WalletFinder(_walletRepository);
    }

    [TestMethod]
    [TestCategory("ReverseDividendCommandHandler tests")]
    public async Task Should_Reverse_Dividend_And_Persist_Wallet()
    {
        _wallet.BuyAsset(_bbas3, 100, 17m);

        _wallet.ReceiveDividend(_bbas3, new Money(0.68m), DateTime.UtcNow);
        _wallet.ReceiveDividend(_bbas3, new Money(0.5m), DateTime.UtcNow); //dividendos a se estornar

        var lastDividend = _wallet.Dividends.Last();
        var command = new ReverseDividendCommand(_wallet.Id, lastDividend.Id);
        var handler = new ReverseDividendCommandHandler(_walletRepository, _walletFinder);
        Money correctDividendsTotal = 100 * 0.68m;

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.AreEqual(correctDividendsTotal, _wallet.GetTotalDividends());
        Assert.IsTrue(_walletRepository.UpdateWasCalled);
        Assert.AreSame(_wallet, _walletRepository.UpdatedWallet);
    }

    [TestMethod]
    [TestCategory("ReverseDividendCommandHandler tests")]
    public async Task Should_Return_Exception_And_Not_Persist_When_Dividend_Is_Not_In_Wallet()
    {
        _wallet.BuyAsset(_bbas3, 100, 17m);

        _wallet.ReceiveDividend(_bbas3, new Money(0.68m), DateTime.UtcNow);

        var command = new ReverseDividendCommand(_wallet.Id, Guid.NewGuid());
        var handler = new ReverseDividendCommandHandler(_walletRepository, _walletFinder);

        await Assert.ThrowsAsync<DividendNotFoundException>
            (async () => await handler.HandleAsync(command, CancellationToken.None));

        Assert.IsFalse(_walletRepository.UpdateWasCalled);
    }
}
