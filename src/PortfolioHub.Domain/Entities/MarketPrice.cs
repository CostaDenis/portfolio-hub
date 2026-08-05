using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class MarketPrice(Money price) : Entity
{
    public Money Price { get; private set; } = price;
    public DateTime LastUpdate { get; private set; } = DateTime.Now;

    public void UpdatePrice(Money newPrice)
    {
        Price = newPrice;
        LastUpdate = DateTime.Now;
    }
}