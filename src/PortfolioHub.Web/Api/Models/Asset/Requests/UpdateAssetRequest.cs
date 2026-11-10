namespace PortfolioHub.Web.Api.Models.Asset.Requests;

public record UpdateAssetRequest(string AssetName, string Ticker, string Type);