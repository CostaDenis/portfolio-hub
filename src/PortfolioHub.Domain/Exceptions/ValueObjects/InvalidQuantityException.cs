namespace PortfolioHub.Domain.Exceptions.ValueObjects;

public class InvalidQuantityException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(decimal value)
    {
        if (value < 0)
            ThrowIf(true, new InvalidQuantityException("A quantidade não pode ser negativa!"));
    }

}