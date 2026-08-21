namespace PortfolioHub.Api.Contracts.Wallets;

public class BuyAssetRequest(Guid assetId, decimal quantity, decimal unitPrice)
{
    public Guid AssetId { get; set; } = assetId;
    public decimal Quantity { get; set; } = quantity;
    public decimal UnitPrice { get; set; } = unitPrice;
}