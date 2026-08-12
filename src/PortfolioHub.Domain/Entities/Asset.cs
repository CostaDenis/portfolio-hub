using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Asset : Entity
{

    public Asset(AssetName name, Ticker ticker,
    EAssetType type, MarketPrice marketPrice)
    {
        InvalidPriceException.ThrowIfInvalid(marketPrice.Price);

        Name = name;
        Ticker = ticker;
        Type = type;
        MarketPrice = marketPrice;
    }

    public AssetName Name { get; private set; }
    public Ticker Ticker { get; private set; }
    public EAssetType Type { get; private set; }
    public MarketPrice MarketPrice { get; private set; }

    public void UpdateName(AssetName assetName)
        => Name = assetName;

    public void UpdateTicker(Ticker ticker)
        => Ticker = ticker;
    public void UpdateType(EAssetType type)
        => Type = type;

}