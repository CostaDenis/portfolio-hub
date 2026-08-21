using PortfolioHub.Api.Endpoints.Abstractions;
using PortfolioHub.Api.Endpoints.Assets;
using PortfolioHub.Api.Endpoints.Wallets;

namespace PortfolioHub.Api.Endpoints;

public static class Endpoint
{

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("HealthCheck")
            .WithSummary("Confere o Status da API")
            .WithDescription("Confere o Status da API")
            .MapGet("/", () => new { message = "Ok" });

        endpoints.MapGroup("v1/assets")
            .WithTags("Assets")
            // .RequireAuthorization()
            .MapEndpoint<GetAssetByIdEndpoint>()
            .MapEndpoint<GetAssetsEndpoint>()
            .MapEndpoint<UpdateAssetEndpoint>()
            .MapEndpoint<UpdateMarketPriceEndpoint>();

        endpoints.MapGroup("v1/wallets")
            .WithTags("Wallets")
            // .RequireAuthorization()
            .MapEndpoint<BuyAssetEndpoint>()
            .MapEndpoint<CreateWalletEndpoint>()
            .MapEndpoint<GetWalletAssetPositionEndpoint>()
            .MapEndpoint<GetWalletByIdEndpoint>()
            .MapEndpoint<GetWalletDividendsEndpoint>()
            .MapEndpoint<GetWalletPositionEndpoint>()
            .MapEndpoint<GetWalletTransactionsEndpoint>()
            .MapEndpoint<ReceiveDividendEndpoint>()
            .MapEndpoint<SellAssetEndpoint>()
            .MapEndpoint<UpdateWalletNameEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}