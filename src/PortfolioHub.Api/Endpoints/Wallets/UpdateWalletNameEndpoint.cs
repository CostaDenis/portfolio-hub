using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Application.Commands.Wallets;
using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Endpoints.Wallets;

public class UpdateWalletNameEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("{walletId:guid}", HandleAsync)
            .WithName("UpdateWallet")
            .WithSummary("Atualiza Carteira")
            .WithDescription("Atualiza Carteira")
            .Produces<WalletDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(Guid walletId, UpdateWalletNameRequest request, UpdateWalletNameCommandHandler handler, CancellationToken cancellationToken)
    {
        var command = new UpdateWalletNameCommand(walletId, new WalletName(request.Name));
        var result = await handler.HandleAsync(command, cancellationToken);

        return TypedResults.Ok(result);
    }
}