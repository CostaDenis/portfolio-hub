namespace PortfolioHub.Application.Exceptions;

public class AssetNotFoundException(string message = "O ativo não foi encontrado!") : Exception(message);