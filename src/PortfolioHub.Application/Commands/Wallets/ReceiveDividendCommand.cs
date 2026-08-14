using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Wallets;

public class ReceiveDividendCommand(Guid walletId, Guid assetId,
 Money valuePerShare, DateTime date)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid AssetId { get; init; } = assetId;
    public Money ValuePerShare { get; init; } = valuePerShare;
    public DateTime Date { get; init; } = date;
}