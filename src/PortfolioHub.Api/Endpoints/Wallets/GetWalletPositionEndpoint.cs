using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletPositionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{walletId:guid}/position", HandleAsync)
            .WithName("GetWalletPosition")
            .WithSummary("Consulta a posição da Carteira")
            .WithDescription("Consulta a posição da Carteira")
            .Produces<IReadOnlyCollection<WalletPositionDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        GetWalletPositionQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetWalletPositionQuery(walletId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return Results.Ok(result);
    }
}