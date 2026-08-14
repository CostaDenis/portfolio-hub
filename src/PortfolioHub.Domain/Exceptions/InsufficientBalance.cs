using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class InsufficientBalance(string message)
    : BaseException(message);