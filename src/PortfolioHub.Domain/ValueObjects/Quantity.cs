using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public Quantity(int value)
    {
        InvalidQuantityException.ThrowIfInvalid(value);
        Value = value;
    }

    public int Value { get; }

    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(int value) => new(value);

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

}