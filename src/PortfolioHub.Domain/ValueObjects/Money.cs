using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class Money(decimal value) : ValueObject
{
    public decimal Value { get; } = value;

    public static implicit operator decimal(Money money) => money.Value;
    public static implicit operator Money(decimal value) => new(value);

    public Money Add(Money money)
        => new(Value + money);

    public Money Subtract(Money money)
        => new(Value - money);

    public Money Multiply(Quantity quantity)
        => new(Value * quantity);

    public Money Divide(Quantity quantity)
    {
        UndeterminedResultException.ThrowIfInvalid(quantity);
        return new(Value / quantity);
    }

}