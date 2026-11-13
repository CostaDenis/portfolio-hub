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
public class ReverseTransactionCommandHandlerTests
{

    private readonly Wallet _wallet;
    private readonly Asset _bbas3;
    private readonly FakeWalletRepository _walletRepository;
    private readonly WalletFinder _walletFinder;

    public ReverseTransactionCommandHandlerTests()
    {
        _wallet = new Wallet(new WalletName("Ações"));
        _bbas3 = new Asset(new AssetName("Banco do Brasil SA"),
            new Ticker("BBAS3"), EAssetType.Stock, new MarketPrice(17.7m));
        _walletRepository = new FakeWalletRepository(_wallet);
        _walletFinder = new WalletFinder(_walletRepository);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionCommandHandler tests")]
    public async Task Should_Reverse_Buy_Transaction_And_Persist_Wallet()
    {
        _wallet.BuyAsset(_bbas3, 100, 16m);
        _wallet.BuyAsset(_bbas3, 15, 16m); //transação a se estornar

        var lastTransaction = _wallet.Transactions.Last();
        var command = new ReverseTransactionCommand(_wallet.Id, lastTransaction.Id);
        var handler = new ReverseTransactionCommandHandler(_walletRepository, _walletFinder);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.AreEqual(100, _wallet.GetCurrentQuantity(_bbas3).Value);
        Assert.IsTrue(_walletRepository.UpdateWasCalled);
        Assert.AreSame(_wallet, _walletRepository.UpdatedWallet);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionCommandHandler tests")]
    public async Task Should_Reverse_Sell_Transaction_And_Persist_Wallet()
    {
        _wallet.BuyAsset(_bbas3, 100, 16m);
        _wallet.SellAsset(_bbas3, 15, 16m); //transação a se estornar

        var lastTransaction = _wallet.Transactions.Last();
        var command = new ReverseTransactionCommand(_wallet.Id, lastTransaction.Id);
        var handler = new ReverseTransactionCommandHandler(_walletRepository, _walletFinder);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.AreEqual(100, _wallet.GetCurrentQuantity(_bbas3).Value);
        Assert.IsTrue(_walletRepository.UpdateWasCalled);
        Assert.AreSame(_wallet, _walletRepository.UpdatedWallet);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionCommandHandler tests")]
    public async Task Should_Return_Exception_And_Not_Persist_When_Reversing_Buy_Already_Sold()
    {
        _wallet.BuyAsset(_bbas3, 100, 16m);
        var lastTransaction = _wallet.Transactions.Last();

        _wallet.SellAsset(_bbas3, 70, 17m);

        var command = new ReverseTransactionCommand(_wallet.Id, lastTransaction.Id);
        var handler = new ReverseTransactionCommandHandler(_walletRepository, _walletFinder);

        await Assert.ThrowsAsync<InsufficientBalanceException>
            (async () => await handler.HandleAsync(command, CancellationToken.None));
        Assert.IsFalse(_walletRepository.UpdateWasCalled);
        Assert.IsNull(_walletRepository.UpdatedWallet);
    }

}
