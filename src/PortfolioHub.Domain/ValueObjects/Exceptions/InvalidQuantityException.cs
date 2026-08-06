namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public class InvalidQuantityException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(int value)
    {
        if (value < 0)
            ThrowIf(true, new InvalidQuantityException("A quantidade não pode ser negativa!"));
    }

}