namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public class InvalidWalletNameException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            ThrowIf(true, new InvalidWalletNameException("O nome da carteira não pode ser nulo!"));

        if (value.Length is < 2 or > 20)
            ThrowIf(true, new InvalidWalletNameException("O nome da carteira deve ter entre 2 e 20 caracteres!"));
    }
}