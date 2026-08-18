using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Commands.Wallets;

[TestClass]
public class CreateWalletCommandHandlerTests
{
    [TestMethod]
    [TestCategory("CreateWalletCommandHandler tests")]
    public async Task Should_Create_And_Persist_Wallet()
    {
        var command = new CreateWalletCommand(new WalletName("Ações"));
        var repository = new FakeWalletRepository();
        var handler = new CreateWalletCommandHandler(repository);

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(repository.CreateWasCalled);
        Assert.IsNotNull(repository.CreatedWallet);
        Assert.AreEqual(new WalletName("Ações"), repository.CreatedWallet.Name);
    }
}