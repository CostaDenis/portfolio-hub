using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Assets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Commands.Assets;

namespace PortfolioHub.Api.Endpoints.Assets;

public class SyncMarketPricesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("prices/sync", HandleAsync)
            .WithName("SyncMarketPrices")
            .WithSummary("Sincroniza os preços dos Ativos")
            .WithDescription("Busca os preços atuais nas APIs de mercado e atualiza os Ativos")
            .Produces<SyncMarketPricesResultDTO>(StatusCodes.Status200OK);

    private static async Task<IResult> HandleAsync(
        SyncMarketPricesCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(new SyncMarketPricesCommand(true), cancellationToken);

        return Results.Ok(result);
    }
}
