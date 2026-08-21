using PortfolioHub.Api.Contracts.Assets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Assets;

public class UpdateMarketPriceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("{assetId:guid}/market-price", HandleAsync)
            .WithName("UpdateMarketPrice")
            .WithSummary("Atualiza o preço do Ativo")
            .WithDescription("Atualiza o preço do Ativo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid assetId,
        UpdateMarketPriceRequest request,
        UpdateMarketPriceCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateMarketPriceCommand(assetId, new MarketPrice(new Money(request.Price)));

        await handler.HandleAsync(command, cancellationToken);

        return Results.NoContent();
    }
}