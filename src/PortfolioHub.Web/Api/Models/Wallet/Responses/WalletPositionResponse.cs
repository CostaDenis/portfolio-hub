namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record WalletPositionResponse(
    Guid AssetId, string AssetName, string Ticker, decimal Quantity, decimal MarketPrice)
{
    public decimal Total => Quantity * MarketPrice;
}