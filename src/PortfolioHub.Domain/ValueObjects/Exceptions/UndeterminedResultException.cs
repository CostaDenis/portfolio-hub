namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public partial class UndeterminedResultException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(decimal value)
    {
        if (value == 0)
            ThrowIf(true, new UndeterminedResultException("Não é possível definir uma divisão com zero!"));
    }
}