namespace PortfolioHub.Application.DTOs;

public class SyncMarketPricesResultDTO(int updated, int notFound, int skipped)
{
    public int Updated { get; init; } = updated;
    public int NotFound { get; init; } = notFound;
    public int Skipped { get; init; } = skipped;

}
