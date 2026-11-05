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
        _wallets.Add(wallet);
    }

    public FakeWalletRepository(IEnumerable<Wallet> wallets)
    {
        _wallets.AddRange(wallets);
    }

    private readonly List<Wallet> _wallets = [];

    public bool CreateWasCalled { get; private set; }
    public Wallet? CreatedWallet { get; private set; }
    public bool UpdateWasCalled { get; private set; }
    public Wallet? UpdatedWallet { get; private set; }

    public Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
        => Task.FromResult(_wallets.FirstOrDefault(wallet => wallet.Id == walletId));

    public Task<IReadOnlyCollection<Wallet>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<Wallet>>(_wallets.AsReadOnly());

    public Task CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        CreateWasCalled = true;
        CreatedWallet = wallet;
        _wallets.Add(wallet);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        UpdateWasCalled = true;
        UpdatedWallet = wallet;
        _wallets.RemoveAll(stored => stored.Id == wallet.Id);
        _wallets.Add(wallet);
        return Task.CompletedTask;
    }
}
