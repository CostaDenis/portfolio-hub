using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class BuyAssetEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("{walletId:guid}/assets/buy", HandleAsync)
            .WithName("BuyAsset")
            .WithSummary("Compra Ativo")
            .WithDescription("Compra Ativo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        BuyAssetRequest request,
        BuyAssetCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new BuyAssetCommand
            (walletId, request.AssetId, new Quantity(request.Quantity), new Money(request.UnitPrice));

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}