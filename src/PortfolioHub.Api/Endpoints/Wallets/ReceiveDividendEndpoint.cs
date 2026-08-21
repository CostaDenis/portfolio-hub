using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class ReceiveDividendEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("{walletId:guid}/assets/receive-dividend", HandleAsync)
            .WithName("ReceiveDividend")
            .WithSummary("Recebe Dividendos")
            .WithDescription("Recebe Dividendos")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        ReceiveDividendRequest request,
        ReceiveDividendCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new ReceiveDividendCommand
            (walletId, request.AssetId, new Money(request.ValuePerShare), request.Date);

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}