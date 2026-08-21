using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class SellAssetEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("{walletId:guid}/assets/sell", HandleAsync)
            .WithName("SellAsset")
            .WithSummary("Vende Ativo")
            .WithDescription("Vende Ativo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        SellAssetRequest request,
        SellAssetCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new SellAssetCommand
            (walletId, request.AssetId, new Quantity(request.Quantity), new Money(request.UnitPrice));

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}