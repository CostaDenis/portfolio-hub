using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.ValueObjects;

public class AssetName : ValueObject
{
    public AssetName(string value)
    {
        InvalidAssetNameException.ThrowIfInvalid(value);
        Value = value.Trim();
    }
    
    public string Value { get; private set; }
    
    public override string ToString() => Value;
    public static implicit operator string(AssetName assetName) => assetName.Value;
    public static implicit operator AssetName(string value) => new(value);
}