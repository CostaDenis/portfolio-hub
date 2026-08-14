using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Application.Commands.Wallets;

public class SellAssetCommand(Guid walletId, Guid assetId,
    Quantity quantity, Money unitPrice)
{
    public Guid WalletId { get; init; } = walletId;
    public Guid AssetId { get; init; } = assetId;
    public Quantity Quantity { get; init; } = quantity;
    public Money UnitPrice { get; init; } = unitPrice;
}
