using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Wallets;

[TestClass]
public class UpdateWalletNameCommandHandlerTests
{
    [TestMethod]
    [TestCategory("UpdateWalletNameCommandHandlers tests")]
    public async Task Should_Update_WalletName_And_Persist_Wallet()
    {
        var wallet = new Wallet(new WalletName("Carteira"));
        var repository = new FakeWalletRepository(wallet);
        var walletFinder = new WalletFinder(repository);
        var handler = new UpdateWalletNameCommandHandler(repository, walletFinder);
        var command = new UpdateWalletNameCommand(wallet.Id, new WalletName("Carteira atualizada"));

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.AreEqual(new WalletName("Carteira atualizada"), wallet.Name);
        Assert.IsTrue(repository.UpdateWasCalled);
        Assert.AreSame(wallet, repository.UpdatedWallet);
    }
}