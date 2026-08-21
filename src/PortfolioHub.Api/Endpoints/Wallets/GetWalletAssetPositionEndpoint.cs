using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletAssetPositionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{walletId:guid}/assets/{assetId:guid}/position", HandleAsync)
            .WithName("GetWalletAssetPosition")
            .WithSummary("Consulta a posição da Carteira por Ativo")
            .WithDescription("Consulta a posição da Carteira por Ativo")
            .Produces<WalletPositionDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        Guid assetId,
        GetWalletAssetPositionQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetWalletAssetPositionQuery(walletId, assetId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return Results.Ok(result);
    }
}