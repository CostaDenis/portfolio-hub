namespace PortfolioHub.Domain.Exceptions.ValueObjects;

public class InvalidTickerException(string message) : BaseException(message)
{

    public static void ThrowIfInvalid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            ThrowIf(true, new InvalidTickerException("O Ticker não pode ser nulo!"));

        if (value.Length is < 2 or > 10)
            ThrowIf(true, new InvalidTickerException("O Ticker deve conter entre 2 e 10 caracteres!"));

        if (!value.All(char.IsLetterOrDigit))
            ThrowIf(true, new InvalidTickerException("O Ticker pode conter apenas letrar e digitos!"));
    }
}