namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record WalletPositionResponse(
    Guid AssetId, string AssetName, string Ticker, decimal Quantity, decimal MarketPrice, decimal AveragePrice)
{
    public decimal Total => Quantity * MarketPrice;

    // Quanto as cotas que estão na carteira custaram.
    public decimal Cost => Quantity * AveragePrice;

    public decimal Result => Total - Cost;

    public decimal ResultPercent => Cost == 0 ? 0 : Result / Cost;
}
