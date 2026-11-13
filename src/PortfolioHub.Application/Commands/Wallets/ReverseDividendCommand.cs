namespace PortfolioHub.Application.Commands.Wallets;

public class ReverseDividendCommand(Guid walletId, Guid dividendId)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid DividendId { get; init; } = dividendId;
}
