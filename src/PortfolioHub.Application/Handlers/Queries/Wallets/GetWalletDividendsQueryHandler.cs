using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletDividendsQueryHandler(WalletFinder walletFinder)
{
    public async Task<WalletDividendsDTO> HandleAsync(
        GetWalletDividendsQuery query,
        CancellationToken cancellationToken)
    {
        var wallet = await walletFinder.GetRequiredAsync(query.WalletId, cancellationToken);

        List<DividendDTO> dividends = [];

        foreach (var dividend in wallet.Dividends)
        {
            dividends.Add(new DividendDTO(
                dividend.Id,
                dividend.Asset.Id,
                dividend.Asset.Ticker,
                dividend.Quantity,
                dividend.ValuePerShare,
                dividend.Date,
                dividend.Total));
        }

        return new WalletDividendsDTO(
            wallet.GetTotalDividends(),
            dividends.AsReadOnly());
    }
}
