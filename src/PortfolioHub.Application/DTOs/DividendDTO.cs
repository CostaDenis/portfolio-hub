namespace PortfolioHub.Application.DTOs;

public class DividendDTO(Guid dividendId, Guid assetId,
    string ticker, decimal quantity,
    decimal valuePerShare, DateTime date, decimal total)
{
    public Guid DividendId { get; init; } = dividendId;
    public Guid AssetId { get; init; } = assetId;
    public string Ticker { get; init; } = ticker;
    public decimal Quantity { get; init; } = quantity;
    public decimal ValuePerShare { get; init; } = valuePerShare;
    public DateTime Date { get; init; } = date;
    public decimal Total { get; init; } = total;
}