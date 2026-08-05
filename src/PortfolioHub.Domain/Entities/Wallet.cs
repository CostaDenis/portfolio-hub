using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.Entities;

public class Wallet(WalletName name) : Entity
{
    private readonly List<Transaction> _transactions = [];

    public WalletName Name { get; private set; } = name;
    public IReadOnlyCollection<Transaction> Transactions { get { return _transactions.ToArray(); } }


    public void ChangeName(WalletName walletName)
        => Name = walletName;

    public void BuyAsset(Asset asset, int quantity, Money unitPrice)
            => _transactions.Add(
                    new Transaction(asset, ETransactionType.Buy, quantity, unitPrice));

    public void SellAsset(Asset asset, Quantity quantity, Money unitPrice)
    {
        if (!CanSell(asset, quantity))
            throw new InvalidQuantityException("Não possui quantidade suficiente para vender!");

        var transaction = new Transaction(asset, ETransactionType.Buy, quantity, unitPrice);
        _transactions.Add(transaction);
    }

    private bool CanSell(Asset asset, int quantity)
        => GetCurrentQuantity(asset) >= quantity;

    private Quantity GetCurrentQuantity(Asset asset)
    {
        var quantity = 0;

        foreach (var transaction in GetTransactions(asset))
        {
            if (transaction.IsBuy())
                quantity += transaction.Quantity;
            else if (transaction.IsSell())
                quantity -= transaction.Quantity;
        }
        return quantity;
    }

    public bool ContainsAsset(Asset asset)
        => GetCurrentQuantity(asset) > 0;

    public IReadOnlyCollection<Transaction> GetTransactions(Asset asset)
        => _transactions.Where(x => x.Asset.Id == asset.Id).ToList().AsReadOnly();
}