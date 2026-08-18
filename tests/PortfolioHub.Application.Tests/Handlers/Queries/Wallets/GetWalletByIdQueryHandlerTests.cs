using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Handlers.Queries.Wallets;

[TestClass]
public class GetWalletByIdQueryHandlerTests
{
    [TestMethod]
    [TestCategory("GetWalletByIdQueryHandler tests")]
    public async Task Should_Return_WalletDTO_Without_Changing_Wallet()
    {
        var wallet = new Wallet(new WalletName("Fundos Imobiliários"));
        var repository = new FakeWalletRepository(wallet);
        var finder = new WalletFinder(repository);
        var handler = new GetWalletByIdQueryHandler(finder);
        var query = new GetWalletByIdQuery(wallet.Id);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.AreEqual(wallet.Id, result.WalletId);
        Assert.AreEqual(wallet.Name.Value, result.Name);
        Assert.IsFalse(repository.CreateWasCalled);
        Assert.IsFalse(repository.UpdateWasCalled);
    }
}
