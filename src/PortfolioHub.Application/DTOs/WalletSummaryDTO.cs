namespace PortfolioHub.Application.DTOs;

public class WalletSummaryDTO(Guid walletId, string name, decimal netWorth, int assetCount)
{
    public Guid WalletId { get; init; } = walletId;
    public string Name { get; init; } = name;
    public decimal NetWorth { get; init; } = netWorth;
    public int AssetCount { get; init; } = assetCount;
}
