using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Api.Contracts.Assets;

public class UpdateAssetRequest(string assetName, string ticker, EAssetType type)
{
    public string AssetName { get; init; } = assetName;
    public string Ticker { get; init; } = ticker;
    public EAssetType Type { get; init; } = type;
}