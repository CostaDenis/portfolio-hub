namespace PortfolioHub.Api.Options;

public class PriceSyncOptions
{
    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 30;
}
