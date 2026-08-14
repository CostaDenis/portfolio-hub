namespace PortfolioHub.Application.DTOs;

public class WalletDTO(Guid walletId, string name)
{
    public Guid WalletId { get; init; } = walletId;
    public string Name { get; init; } = name;
}