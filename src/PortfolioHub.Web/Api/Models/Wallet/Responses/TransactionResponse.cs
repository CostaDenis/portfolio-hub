namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record TransactionResponse(
    Guid TransactionId, Guid AssetId, string Ticker, string Type,
    DateTime Date, decimal Quantity, decimal UnitPrice, decimal Total)
{
    public bool IsBuy => Type == "Buy";
}
