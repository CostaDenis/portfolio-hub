namespace PortfolioHub.Application.DTOs;

public class WalletDividendsDTO(decimal totalReceived, IReadOnlyCollection<DividendDTO> dividends)
{
    public decimal TotalReceived { get; init; } = totalReceived;
    public IReadOnlyCollection<DividendDTO> Dividends { get; init; } = dividends;
}