using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class InvalidPriceException(string message)
    : BaseException(message)
{
    public static void ThrowIfInvalid(decimal price)
    {
        if (price < 0)
            ThrowIf(true, new InvalidPriceException("O preço do Ativo não pode ser negativo!"));
    }

    public static void ThrowIfNotPositive(decimal price, string message)
        => ThrowIf(price <= 0, new InvalidPriceException(message));
}
