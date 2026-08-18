using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Wallet(WalletName name) : Entity
{
    private readonly List<Transaction> _transactions = [];
    private readonly List<Dividend> _dividends = [];

    public WalletName Name { get; private set; } = name;
    public IReadOnlyCollection<Transaction> Transactions { get { return _transactions.ToArray(); } }
    public IReadOnlyCollection<Dividend> Dividends { get { return _dividends.ToArray(); } }

    public void UpdateName(WalletName walletName)
        => Name = walletName;

    public void BuyAsset(Asset asset, Quantity quantity, Money unitPrice)
            => _transactions.Add(
                    new Transaction(asset, ETransactionType.Buy, quantity, unitPrice));

    public void SellAsset(Asset asset, Quantity quantity, Money unitPrice)
    {
        if (!CanSell(asset, quantity))
            throw new InsufficientBalanceException("Não possui quantidade suficiente para vender!");

        var transaction = new Transaction(asset, ETransactionType.Sell, quantity, unitPrice);
        _transactions.Add(transaction);
    }

    private bool CanSell(Asset asset, Quantity quantity)
        => GetCurrentQuantity(asset) >= quantity;

    public Quantity GetCurrentQuantity(Asset asset)
    {
        Quantity quantity = 0;

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

    public void ReceiveDividend(Asset asset, Money valuePerShare, DateTime date)
    {
        if (!ContainsAsset(asset))
            throw new InsufficientAssetQuantityException(
                "Não possui quantidade suficiente para receber dividendo!");

        _dividends.Add(new Dividend(
            asset, GetCurrentQuantity(asset), valuePerShare, date));
    }

    public IReadOnlyCollection<Dividend> GetDividendsByAsset(Asset asset)
        => _dividends.Where(x => x.Asset.Id == asset.Id).ToList().AsReadOnly();

    public Money GetTotalDividendsByAsset(Asset asset)
    {
        Money totalReceived = 0;
        foreach (var dividend in _dividends)
        {
            if (dividend.Asset.Id == asset.Id)
                totalReceived += dividend.Total;
        }

        return totalReceived;
    }

    public Money GetTotalDividends()
    {
        Money totalReceived = 0;
        foreach (var dividend in _dividends)
            totalReceived += dividend.Total;

        return totalReceived;
    }
}
