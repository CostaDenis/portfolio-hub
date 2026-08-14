using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.Queries.Wallets;

public class GetWalletTransactionsQuery(Guid walletId, Guid? assetId,
    ETransactionType? type, DateTime? startDate, DateTime? endDate)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid? AssetId { get; init; } = assetId;
    public ETransactionType? Type { get; init; } = type;
    public DateTime? StartDate { get; init; } = startDate;
    public DateTime? EndDate { get; init; } = endDate;
}