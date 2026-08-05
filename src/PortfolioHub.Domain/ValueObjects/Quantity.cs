using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public Quantity(int value)
    {
        InvalidQuantityException.ThrowIfInvalid(value);
        Value = value;
    }

    public int Value { get; private set; }

    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(int value) => new(value);

    public void Increase(Quantity quantity)
        => Value += quantity;

    public bool CanDecrease(Quantity quantity)
    {
        decimal result = Value - quantity;
        return result >= 0 ? true : false;
    }

    public void Decrease(Quantity quantity)
        => Value += quantity;
}