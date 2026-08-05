using System.Text.RegularExpressions;

namespace PortfolioHub.Domain.ValueObjects.Exceptions;

public partial class InvalidEmailException(string message = InvalidEmailException.DefaultErrorMessage)
    : BaseException(message)
{
    private const string DefaultErrorMessage = "Email inválido!";
    private static readonly Regex EmailRegex = Regex();
    
    public static void ThrowIfInvalid(string value, string message = InvalidEmailException.DefaultErrorMessage)
        => ThrowIf(string.IsNullOrWhiteSpace(value) 
                   || !EmailRegex.IsMatch(value), new InvalidEmailException(message));
    

    [GeneratedRegex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase 
            | RegexOptions.Compiled, "pt-BR")]
    private static partial Regex Regex();
}