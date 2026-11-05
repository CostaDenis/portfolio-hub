using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Transaction : Entity
{

    private Transaction()
    {
        Asset = null!;
        Quantity = null!;
        UnitPrice = null!;
    }

    public Transaction(Asset asset, ETransactionType type,
    Quantity quantity, Money unitPrice)
    {
        InvalidQuantityException.ThrowIfNotPositive(quantity,
            "A quantidade da transação deve ser maior que zero!");

        InvalidPriceException.ThrowIfNotPositive(unitPrice,
            "O preço unitário deve ser maior que zero!");

        Asset = asset;
        Type = type;
        Date = DateTime.UtcNow;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Asset Asset { get; private set; }
    public ETransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Total => UnitPrice * Quantity;

    public bool IsBuy()
        => Type == ETransactionType.Buy;

    public bool IsSell()
        => Type == ETransactionType.Sell;

}