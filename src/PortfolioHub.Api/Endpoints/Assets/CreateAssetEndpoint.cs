using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Assets;

public class CreateAssetEndpoint : IEndpoint
{

    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("", HandleAsync)
                .WithName("CreateAsset")
                .WithSummary("Cria Ativo")
                .WithDescription("Cria Ativo")
                .Produces<AssetDTO>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        CreateAssetRequest request,
        CreateAssetCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssetCommand(new AssetName(request.AssetName), new Ticker(request.Ticker),
            request.Type, new MarketPrice(request.MarketPrice));
        var result = await handler.HandleAsync(command, cancellationToken);

        return Results.Created($"/v1/assets/{result.AssetId}", result);
    }
}
