namespace PortfolioHub.Api.Contracts.Wallets;

public class ReceiveDividendRequest(Guid assetId, decimal valuePerShare, DateTime date)
{
    public Guid AssetId { get; init; } = assetId;
    public decimal ValuePerShare { get; init; } = valuePerShare;
    public DateTime Date { get; init; } = date;
}