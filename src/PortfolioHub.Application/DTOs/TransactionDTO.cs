using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Application.DTOs;

public class TransactionDTO(Guid transactionId, Guid assetId, string ticker,
    ETransactionType type, DateTime date, decimal quantity, decimal unitPrice, decimal total)
{
    public Guid TransactionId { get; init; } = transactionId;
    public Guid AssetId { get; init; } = assetId;
    public string Ticker { get; init; } = ticker;
    public string Type { get; init; } = type.ToString();
    public DateTime Date { get; init; } = date;
    public decimal Quantity { get; init; } = quantity;
    public decimal UnitPrice { get; init; } = unitPrice;
    public decimal Total { get; init; } = total;
}