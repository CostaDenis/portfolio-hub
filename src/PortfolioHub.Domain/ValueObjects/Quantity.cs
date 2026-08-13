using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public Quantity(decimal value)
    {
        InvalidQuantityException.ThrowIfInvalid(value);
        Value = value;
    }

    public decimal Value { get; }

    public static implicit operator decimal(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(decimal value) => new(value);

    public Quantity Increase(Quantity quantity)
        => new(Value + quantity);

    public bool CanDecrease(Quantity quantity)
        => Value >= quantity;

    public Quantity Decrease(Quantity quantity)
    {
        if (!CanDecrease(quantity))
            InsufficientQuantityException.ThrowIfInvalid(Value, quantity.Value);

        return new Quantity(Value - quantity);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}