using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{walletId:guid}", HandleAsync)
            .WithName("GetWalletById")
            .WithSummary("Consulta Carteira")
            .WithDescription("Consulta Carteira")
            .Produces<WalletDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        GetWalletByIdQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetWalletByIdQuery(walletId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return Results.Ok(result);
    }
}