namespace PortfolioHub.Web.Api.Models.Asset.Responses;

public record AssetResponse(Guid AssetId,
    string Name, string Ticker, string Type, decimal Price);