using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Transaction(Asset asset, ETransactionType type,
    int quantity, Money unitPrice) : Entity
{
    public Asset Asset { get; private set; } = asset;
    public ETransactionType Type { get; private set; } = type;
    public DateTime Date { get; private set; } = DateTime.Now;
    public int Quantity { get; private set; } = quantity;
    public Money UnitPrice { get; private set; } = unitPrice;
    public Money Total => UnitPrice * Quantity;

    public bool IsBuy()
        => Type == ETransactionType.Buy;

    public bool IsSell()
        => Type == ETransactionType.Sell;

    public bool IsDividend()
        => Type == ETransactionType.Dividend;
}