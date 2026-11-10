namespace PortfolioHub.Web.Api.Models.Wallet.Requests;

public record TradeAssetRequest(Guid AssetId, decimal Quantity, decimal UnitPrice);
