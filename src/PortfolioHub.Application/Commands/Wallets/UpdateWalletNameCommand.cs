using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Wallets;

public class UpdateWalletNameCommand(Guid walletId, WalletName walletName)
{
    public Guid WalletId { get; init; } = walletId;
    public WalletName WalletName { get; init; } = walletName;
}