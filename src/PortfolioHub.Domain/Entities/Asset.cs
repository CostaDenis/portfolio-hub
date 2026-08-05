using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Asset(AssetName name, Ticker ticker, EAssetType type) : Entity
{
    public AssetName Name { get; private set; } = name;
    public Ticker Ticker { get; private set; } = ticker;
    public EAssetType Type { get; private set; } = type;
    public MarketPrice MarketPrice { get; private set; } = new MarketPrice(new Money(0));

    public void UpdateName(AssetName assetName)
        => Name = assetName;

    public void UpdateTickerName(Ticker ticker)
        => Ticker = ticker;
    public void UpdateType(EAssetType type)
        => Type = type;

}