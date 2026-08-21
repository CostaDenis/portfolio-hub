using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class CreateWalletEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("", HandleAsync)
            .WithName("CreateWallet")
            .WithSummary("Cria Carteira")
            .WithDescription("Cria Carteira")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
       CreateWalletRequest request,
       CreateWalletCommandHandler handler,
       CancellationToken cancellationToken)
    {
        var command = new CreateWalletCommand(
            new WalletName(request.Name));

        var result = await handler.HandleAsync(command, cancellationToken);

        return Results.Created($"v1/wallets/{result.WalletId}", result);
    }
}