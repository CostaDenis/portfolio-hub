namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record DividendResponse(
    Guid DividendId, Guid AssetId, string Ticker, decimal Quantity,
    decimal ValuePerShare, DateTime Date, decimal Total);
