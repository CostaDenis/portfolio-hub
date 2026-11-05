using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Dividend : Entity
{

    private Dividend()
    {
        Asset = null!;
        Quantity = null!;
        ValuePerShare = null!;
    }

    public Dividend(Asset asset, Quantity quantity,
        Money valuePerShare, DateTime date)
    {
        InvalidQuantityException.ThrowIfNotPositive(quantity,
            "Deve ter pelo menos uma unidade para receber dividendo!");

        InvalidPriceException.ThrowIfNotPositive(valuePerShare,
            "O valor do dividendo deve ser positivo!");

        Asset = asset;
        Quantity = quantity;
        ValuePerShare = valuePerShare;
        Date = date;
    }

    public Asset Asset { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money ValuePerShare { get; private set; }
    public DateTime Date { get; private set; }
    public Money Total => Quantity * ValuePerShare;
}