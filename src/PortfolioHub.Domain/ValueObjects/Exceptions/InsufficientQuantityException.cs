namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public class InsufficientQuantityException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(int value, int newValue)
    {
        if (!(value >= newValue))
            ThrowIf(true, new InsufficientQuantityException("Não é possível subtrair!"));
    }

}