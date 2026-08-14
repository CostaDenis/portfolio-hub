namespace PortfolioHub.Application.DTOs;

public class WalletPositionDTO(Guid assetId, string ticker, string assetName, decimal quantity, decimal marketPrice)
{
    public Guid AssetId { get; init; } = assetId;
    public string Ticker { get; init; } = ticker;
    public string AssetName { get; init; } = assetName;
    public decimal Quantity { get; init; } = quantity;
    public decimal MarketPrice { get; init; } = marketPrice;
}