namespace PortfolioHub.Application.Exceptions;

public class WalletNotFoundException(string message = "A carteira não foi encontrada!")
    : Exception(message);