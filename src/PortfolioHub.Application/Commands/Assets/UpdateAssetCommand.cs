using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Assets;

public class UpdateAssetCommand(Guid assetId, AssetName assetName,
    Ticker ticker, EAssetType type)
{
    public Guid AssetId { get; init; } = assetId;
    public AssetName AssetName { get; init; } = assetName;
    public Ticker Ticker { get; init; } = ticker;
    public EAssetType Type { get; init; } = type;
}