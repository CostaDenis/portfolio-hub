using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Assets;

namespace PortfolioHub.Api.Endpoints.Assets;

public class GetAssetsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("", HandleAsync)
            .WithName("GetAllAssets")
            .WithSummary("Consulta todos os Ativos")
            .WithDescription("Consulta todos os Ativos")
            .Produces<List<AssetDTO>>(StatusCodes.Status200OK);

    private static async Task<IResult> HandleAsync(
        GetAssetsQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(cancellationToken);
        return Results.Ok(result);
    }
}