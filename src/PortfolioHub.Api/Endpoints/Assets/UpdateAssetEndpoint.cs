using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Assets;

public class UpdateAssetEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("{assetId:guid}", HandleAsync)
            .WithName("UpdateAsset")
            .WithSummary("Atualiza Ativo")
            .WithDescription("Atualiza Ativo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid assetId,
        UpdateAssetRequest request,
        UpdateAssetCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateAssetCommand
            (assetId, new AssetName(request.AssetName), new Ticker(request.Ticker), request.Type);

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}