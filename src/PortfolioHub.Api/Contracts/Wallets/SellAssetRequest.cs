namespace PortfolioHub.Api.Contracts.Wallets;

public class SellAssetRequest(Guid assetId, decimal quantity, decimal unitPrice)
{
    public Guid AssetId { get; init; } = assetId;
    public decimal Quantity { get; init; } = quantity;
    public decimal UnitPrice { get; init; } = unitPrice;
}