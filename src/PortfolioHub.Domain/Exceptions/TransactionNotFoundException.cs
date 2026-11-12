using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Domain.Exceptions;

public class TransactionNotFoundException(string message = "A movimentação não foi encontrada!")
    : BaseException(message);
