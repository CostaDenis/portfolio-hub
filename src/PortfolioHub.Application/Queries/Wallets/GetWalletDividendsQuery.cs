namespace PortfolioHub.Application.Queries.Wallets;

public class GetWalletDividendsQuery(Guid walletId)
{
    public Guid WalletId { get; init; } = walletId;
}