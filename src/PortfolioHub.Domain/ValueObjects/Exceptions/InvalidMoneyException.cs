namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public partial class InvalidMoneyException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(decimal value)
    {
        if (value < 0)
            ThrowIf(true, new InvalidMoneyException("O valor não pode ser negativo!"));
    }
}