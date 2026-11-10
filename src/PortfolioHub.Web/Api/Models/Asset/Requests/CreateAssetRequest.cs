namespace PortfolioHub.Web.Api.Models.Asset.Requests;

public record CreateAssetRequest(string AssetName,
    string Ticker, string Type, decimal MarketPrice);