using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletByIdQueryHandler(WalletFinder walletFinder)
{

    public async Task<WalletDTO> HandleAsync(GetWalletByIdQuery query, CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(query.WalletId, cancellationToken);

        return new WalletDTO(query.WalletId, wallet.Name);
    }
}