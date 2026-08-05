using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class Money : ValueObject
{

    public Money(decimal value)
    {
        Value = value;
        InvalidMoneyException.ThrowIfInvalid(value);
    }

    public decimal Value { get; private set; }

    public static implicit operator decimal(Money money) => money.Value;
    public static implicit operator Money(decimal value) => new(value);

    public void Add(Money money)
        => Value += money;

    public void Subtract(Money money)
        => Value -= money;

    public void Multiply(Money money)
        => Value *= money;
    public void Divide(Money money)
        => Value /= money;

}