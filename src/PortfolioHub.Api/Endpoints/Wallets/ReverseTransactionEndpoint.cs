using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class ReverseTransactionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost(
            "{walletId:guid}/transactions/{transactionId:guid}/reverse", HandleAsync)
            .WithName("ReverseTransaction")
            .WithSummary("Estorna transação")
            .WithDescription("Estorna transação")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        Guid transactionId,
        ReverseTransactionCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new ReverseTransactionCommand(walletId, transactionId);

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
