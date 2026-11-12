using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class DividendNotFoundException(string message = "Dividendo não encontrado!")
    : BaseException(message);