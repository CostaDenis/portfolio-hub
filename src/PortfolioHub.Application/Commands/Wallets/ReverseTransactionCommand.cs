namespace PortfolioHub.Application.Commands.Wallets;

public class ReverseTransactionCommand(Guid walletId, Guid transactionId)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid TransactionId { get; init; } = transactionId;
}
