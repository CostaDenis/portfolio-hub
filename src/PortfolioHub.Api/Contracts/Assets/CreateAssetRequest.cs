using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Api.Contracts.Assets;

public class CreateAssetRequest(string assetName, string ticker,
    EAssetType type, decimal marketPrice)
{
    public string AssetName { get; init; } = assetName;
    public string Ticker { get; init; } = ticker;
    public EAssetType Type { get; init; } = type;
    public decimal MarketPrice { get; init; } = marketPrice;
}
