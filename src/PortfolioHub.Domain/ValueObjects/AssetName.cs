using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.ValueObjects;

public class AssetName : ValueObject
{
    public AssetName(string value)
    {
        InvalidAssetNameException.ThrowIfInvalid(value);
        Value = value;
    }

    public string Value { get; private set; }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(AssetName assetName) => assetName.Value;
    public static implicit operator AssetName(string value) => new(value);


}