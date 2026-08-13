using PortfolioHub.Domain.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class MarketPrice : ValueObject
{
    public MarketPrice(Money price)
    {
        InvalidPriceException.ThrowIfInvalid(price);
        Price = price;
    }

    public Money Price { get; private set; }
    public DateTime LastUpdate { get; private set; } = DateTime.UtcNow;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Price;
    }
}