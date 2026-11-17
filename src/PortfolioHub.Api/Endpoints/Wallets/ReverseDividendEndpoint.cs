using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class ReverseDividendEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("{walletId:guid}/dividends/{dividendId:guid}/reverse", HandleAsync)
            .WithName("ReverseDividend")
            .WithSummary("Estorna dividendo")
            .WithDescription("Estorna dividendo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        Guid dividendId,
        ReverseDividendCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new ReverseDividendCommand(walletId, dividendId);

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
