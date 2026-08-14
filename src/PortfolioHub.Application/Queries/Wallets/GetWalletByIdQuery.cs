namespace PortfolioHub.Application.Queries.Wallets;

public class GetWalletByIdQuery(Guid walletId)
{
    public Guid WalletId { get; init; } = walletId;
}
