using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Assets;
using PortfolioHub.Application.Queries.Assets;

namespace PortfolioHub.Api.Endpoints.Assets;

public class GetAssetByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{assetId:guid}", HandleAsync)
            .WithName("GetAsset")
            .WithSummary("Consulta Ativo")
            .WithDescription("Consulta Ativo")
            .Produces<AssetDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid assetId,
        GetAssetByIdQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetAssetByIdQuery(assetId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}