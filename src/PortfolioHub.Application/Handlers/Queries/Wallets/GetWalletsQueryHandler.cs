using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Repositories;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletsQueryHandler(IWalletRepository walletRepository)
{
    public async Task<IReadOnlyCollection<WalletSummaryDTO>> HandleAsync(CancellationToken cancellationToken)
    {
        var wallets = await walletRepository.GetAllAsync(cancellationToken);

        return wallets
            .Select(wallet => new WalletSummaryDTO(
                wallet.Id,
                wallet.Name,
                wallet.GetNetWorth(),
                wallet.GetHeldAssets().Count))
            .OrderBy(wallet => wallet.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList()
            .AsReadOnly();
    }
}
