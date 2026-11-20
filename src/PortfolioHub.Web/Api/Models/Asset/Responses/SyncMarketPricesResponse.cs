namespace PortfolioHub.Web.Api.Models.Asset.Responses;

public record SyncMarketPricesResponse(int Updated, int NotFound, int Skipped);
