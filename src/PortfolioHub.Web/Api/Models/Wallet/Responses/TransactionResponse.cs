namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record TransactionResponse(
    Guid TransactionId, Guid AssetId, string Ticker, string Type,
    DateTime Date, decimal Quantity, decimal UnitPrice, decimal Total, DateTime? ReversedAt)
{
    public bool IsBuy => Type == "Buy";
    public bool IsReversed => ReversedAt is not null;
}
