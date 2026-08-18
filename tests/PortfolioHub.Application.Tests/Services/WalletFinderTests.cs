using PortfolioHub.Application.Exceptions;
using PortfolioHub.Application.Services;
using PortfolioHub.Application.Tests.Repositories;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Tests.Services;

[TestClass]
public class WalletFinderTests
{

    private readonly Wallet _wallet;
    private readonly FakeWalletRepository _repository;
    private readonly WalletFinder _finder;

    public WalletFinderTests()
    {
        _wallet = new Wallet(new WalletName("FIIs"));
        _repository = new FakeWalletRepository(_wallet);
        _finder = new WalletFinder(_repository);
    }

    [TestMethod]
    [TestCategory("WalletFinder tests")]
    public void Should_Return_Exception_When_NotFound_Wallet()
        => Assert.ThrowsAsync<WalletNotFoundException>
            (async () => await _finder.GetRequiredAsync(Guid.NewGuid(), CancellationToken.None));

    [TestMethod]
    [TestCategory("WalletFinder tests")]
    public async Task Should_Return_Wallet()
    {
        var foundWallet = await _finder.GetRequiredAsync(_wallet.Id, CancellationToken.None);

        Assert.AreSame(foundWallet, _wallet);
    }
}