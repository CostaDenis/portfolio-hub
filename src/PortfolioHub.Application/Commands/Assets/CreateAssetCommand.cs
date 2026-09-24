using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Assets;

public class CreateAssetCommand(AssetName assetName, Ticker ticker,
    EAssetType type, MarketPrice marketPrice)
{
    public AssetName AssetName { get; init; } = assetName;
    public Ticker Ticker { get; init; } = ticker;
    public EAssetType Type { get; init; } = type;
    public MarketPrice MarketPrice { get; init; } = marketPrice;
}
