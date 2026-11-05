using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("", HandleAsync)
            .WithName("GetWallets")
            .WithSummary("Lista as Carteiras")
            .WithDescription("Lista as Carteiras com patrimônio atual e quantidade de ativos")
            .Produces<IReadOnlyCollection<WalletSummaryDTO>>(StatusCodes.Status200OK);

    private static async Task<IResult> HandleAsync(
        GetWalletsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(cancellationToken);

        return Results.Ok(result);
    }
}
