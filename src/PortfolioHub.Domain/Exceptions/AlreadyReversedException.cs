using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class AlreadyReversedException(string message = "Movimentação já foi estornada!")
    : BaseException(message);