using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Application.Exceptions;

public class TickerAlreadyUsedException(string message = "Esse Ticker já está em uso!")
    : BaseException(message);
