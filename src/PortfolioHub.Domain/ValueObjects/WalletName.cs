using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.ValueObjects;

public class WalletName : ValueObject
{

    public WalletName(string value)
    {
        InvalidWalletNameException.ThrowIfInvalid(value);
        Value = value;
    }

    public string Value { get; private set; }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(WalletName walletName) => walletName.Value;
    public static implicit operator WalletName(string value) => new(value);
}