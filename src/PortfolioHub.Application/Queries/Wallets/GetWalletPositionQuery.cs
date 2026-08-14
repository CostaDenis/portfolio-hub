namespace PortfolioHub.Application.Queries.Wallets;

public class GetWalletPositionQuery(Guid walletId)
{
    public Guid WalletId { get; init; } = walletId;
}