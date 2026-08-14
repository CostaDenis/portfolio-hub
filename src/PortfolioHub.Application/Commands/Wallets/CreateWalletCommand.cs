using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Wallets;

public class CreateWalletCommand(WalletName name)
{
    public WalletName Name { get; init; } = name;
}
