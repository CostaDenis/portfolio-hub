using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Tests.Repositories;

public class FakeWalletRepository : IWalletRepository
{

    public FakeWalletRepository()
    {
    }

    public FakeWalletRepository(Wallet wallet)
    {
        _wallet = wallet;
    }

    private Wallet? _wallet;

    public bool CreateWasCalled { get; private set; }
    public Wallet? CreatedWallet { get; private set; }
    public bool UpdateWasCalled { get; private set; }
    public Wallet? UpdatedWallet { get; private set; }

    public Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
        => Task.FromResult(_wallet?.Id == walletId
                ? _wallet
                : null);

    public Task CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        CreateWasCalled = true;
        CreatedWallet = wallet;
        _wallet = wallet;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        UpdateWasCalled = true;
        UpdatedWallet = wallet;
        _wallet = wallet;
        return Task.CompletedTask;
    }
}