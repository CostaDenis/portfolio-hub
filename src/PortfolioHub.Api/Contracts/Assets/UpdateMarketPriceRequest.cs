namespace PortfolioHub.Api.Contracts.Assets;

public class UpdateMarketPriceRequest(decimal price)
{
    public decimal Price { get; init; } = price;
}