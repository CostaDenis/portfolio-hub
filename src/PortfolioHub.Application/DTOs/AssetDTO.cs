using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.DTOs;

public class AssetDTO(Guid assetId, string name, string ticker,
    DateTime lastUpdate, EAssetType type, decimal price)
{
    public Guid AssetId { get; init; } = assetId;
    public string Name { get; init; } = name;
    public string Ticker { get; init; } = ticker;
    public DateTime LastUpdate { get; init; } = lastUpdate;
    public string Type { get; init; } = type.ToString();
    public decimal Price { get; init; } = price;
}