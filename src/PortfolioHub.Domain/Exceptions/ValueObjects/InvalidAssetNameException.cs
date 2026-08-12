namespace PortfolioHub.Domain.Exceptions.ValueObjects;

public class InvalidAssetNameException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            ThrowIf(true, new InvalidAssetNameException("O nome do ativo não pode ser vazio!"));

        if (value.Length is < 3 or > 20)
            ThrowIf(true, new InvalidAssetNameException("O nome do ativo deve ter entre 3 e 20 caracteres!"));
    }
}