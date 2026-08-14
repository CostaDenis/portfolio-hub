using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class InsufficientAssetQuantityException(string message)
    : BaseException(message);
