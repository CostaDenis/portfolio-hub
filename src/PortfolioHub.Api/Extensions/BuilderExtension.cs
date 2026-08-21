using System.Text.Json.Serialization;
using PortfolioHub.Api.Exceptions;
using PortfolioHub.Application.Handlers.Commands.Assets;
using PortfolioHub.Application.Handlers.Commands.Wallets;
using PortfolioHub.Application.Handlers.Queries.Assets;
using PortfolioHub.Application.Handlers.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Infrastructure;

namespace PortfolioHub.Api.Extensions;

public static class BuilderExtension
{

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        //Infrastructure service
        builder.Services.AddInfrastructure(builder.Configuration);


        //Application services
        builder.Services.AddScoped<WalletFinder>();
        builder.Services.AddScoped<AssetFinder>();


        //Api services
        builder.Services.AddScoped<UpdateAssetCommandHandler>();
        builder.Services.AddScoped<UpdateMarketPriceCommandHandler>();
        builder.Services.AddScoped<GetAssetByIdQueryHandler>();
        builder.Services.AddScoped<GetAssetsQueryHandler>();

        builder.Services.AddScoped<BuyAssetCommandHandler>();
        builder.Services.AddScoped<CreateWalletCommandHandler>();
        builder.Services.AddScoped<ReceiveDividendCommandHandler>();
        builder.Services.AddScoped<SellAssetCommandHandler>();
        builder.Services.AddScoped<UpdateWalletNameCommandHandler>();
        builder.Services.AddScoped<GetWalletAssetPositionQueryHandler>();
        builder.Services.AddScoped<GetWalletByIdQueryHandler>();
        builder.Services.AddScoped<GetWalletDividendsQueryHandler>();
        builder.Services.AddScoped<GetWalletPositionQueryHandler>();
        builder.Services.AddScoped<GetWalletTransactionsQueryHandler>();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }

    public static void AddHttpJsonOptions(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}