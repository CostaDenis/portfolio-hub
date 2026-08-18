using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class InsufficientBalanceException(string message)
    : BaseException(message);