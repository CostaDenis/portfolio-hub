using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class InsufficientBalance(string message)
    : BaseException(message)
{
    public static void ThrowIfInvalid(decimal price)
    {
        if (price < 0)
            ThrowIf(true, new InsufficientBalance("O preço do Ativo não pode ser negativo!"));
    }
}