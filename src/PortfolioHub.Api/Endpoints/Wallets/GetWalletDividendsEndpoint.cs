using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Queries.Wallets;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class GetWalletDividendsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{walletId:guid}/dividends", HandleAsync)
            .WithName("GetWalletDividends")
            .WithSummary("Consulta os Dividendos da Carteira")
            .WithDescription("Consulta os Dividendos da Carteira")
            .Produces<WalletDividendsDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid walletId,
        GetWalletDividendsQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetWalletDividendsQuery(walletId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return Results.Ok(result);
    }
}