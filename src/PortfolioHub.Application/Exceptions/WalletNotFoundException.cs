namespace PortfolioHub.Application.Exceptions;

public class WalletNotFoundException(string message = "A carteira não foi econtrada!") : Exception(message);