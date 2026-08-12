using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class MarketPrice : Entity
{

    public MarketPrice(Money price)
    {
        InvalidPriceException.ThrowIfInvalid(price);
        Price = price;
    }

    public Money Price { get; private set; }
    public DateTime LastUpdate { get; private set; } = DateTime.Now;

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice < 0.0m)
            InvalidPriceException.ThrowIfInvalid(newPrice);

        Price = newPrice;
        LastUpdate = DateTime.Now;
    }
}