using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Assets;

public class UpdateMarketPriceCommand(Guid assetId, MarketPrice marketPrice)
{
    public Guid AssetId { get; init; } = assetId;
    public MarketPrice MarketPrice { get; init; } = marketPrice;
}