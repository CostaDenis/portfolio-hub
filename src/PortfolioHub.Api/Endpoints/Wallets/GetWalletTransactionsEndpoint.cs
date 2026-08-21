using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Domain.Enums;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletTransactionsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{walletId:guid}/transactions", HandleAsync)
            .WithName("GetWalletTransactions")
            .WithSummary("Consulta as Transações da Carteira")
            .WithDescription("Consulta as Transações da Carteira")
            .Produces<IReadOnlyCollection<TransactionDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
       Guid walletId,
       Guid? assetId,
       ETransactionType? type,
       DateTime? startDate,
       DateTime? endDate,
       GetWalletTransactionsQueryHandler handler,
       CancellationToken cancellationToken)
    {
        var query = new GetWalletTransactionsQuery(walletId, assetId, type, startDate, endDate);

        var result = await handler.HandleAsync(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
