namespace PortfolioHub.Application.DTOs;

public class SyncMarketPricesResultDTO(int updated, int notFound)
{
    public int Updated { get; init; } = updated;
    public int NotFound { get; init; } = notFound;
}
