namespace PortfolioHub.Application.Queries.Wallets;

public class GetWalletAssetPositionQuery(Guid walletId, Guid assetId)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid AssetId { get; init; } = assetId;
}