using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.ValueObjects;

public class Ticker : ValueObject
{

    public Ticker(string value)
    {
        value = value.Trim().ToUpperInvariant();
        InvalidTickerException.ThrowIfInvalid(value);
        Value = value;
    }

    public string Value { get; private set; }

    public override string ToString() => Value;
    public static implicit operator string(Ticker ticker) => ticker.Value;
    public static implicit operator Ticker(string value) => new(value);
}