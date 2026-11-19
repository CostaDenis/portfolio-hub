namespace PortfolioHub.Application.Commands.Assets;

public class SyncMarketPricesCommand(bool force = false)
{
    public bool Force { get; init; } = force;
}
