using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Entities;

public class Wallet(WalletName name) : Entity
{
    private readonly List<Transaction> _transactions = [];
    private readonly List<Dividend> _dividends = [];

    private IEnumerable<Transaction> ActiveTransactions => _transactions.Where(x => x.IsReversed == false);
    private IEnumerable<Dividend> ActiveDividends => _dividends.Where(x => x.IsReversed == false);

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
        decimal quantity = 0;

        foreach (var transaction in GetTransactions(asset).Where(x => x.IsReversed == false))
        {
            if (transaction.IsBuy())
                quantity += transaction.Quantity.Value;
            else if (transaction.IsSell())
                quantity -= transaction.Quantity.Value;
        }
        return new Quantity(quantity);
    }

    public bool ContainsAsset(Asset asset)
        => GetCurrentQuantity(asset) > 0;

    // Ativos com quantidade atual maior que zero (exclui os que já foram vendidos por completo).
    public IReadOnlyCollection<Asset> GetHeldAssets()
        => ActiveTransactions
            .Select(transaction => transaction.Asset)
            .DistinctBy(asset => asset.Id)
            .Where(ContainsAsset)
            .ToList()
            .AsReadOnly();

    // Valor atual da carteira: soma de quantidade × preço de mercado de cada ativo.
    public Money GetNetWorth()
    {
        decimal netWorth = 0;
        foreach (var asset in GetHeldAssets())
            netWorth += GetCurrentQuantity(asset).Value * asset.MarketPrice.Price.Value;

        return netWorth;
    }

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
        foreach (var dividend in ActiveDividends)
        {
            if (dividend.Asset.Id == asset.Id)
                totalReceived += dividend.Total;
        }

        return totalReceived;
    }

    public Money GetTotalDividends()
    {
        Money totalReceived = 0;
        foreach (var dividend in ActiveDividends)
            totalReceived += dividend.Total;

        return totalReceived;
    }

    // É a partir da própria carteira para fazer o estorno
    public void ReverseTransaction(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(x => x.Id == transactionId)
            ?? throw new TransactionNotFoundException();

        if (transaction.IsReversed)
            throw new AlreadyReversedException();

        if (transaction.IsBuy()
            && GetCurrentQuantity(transaction.Asset) < transaction.Quantity)
            throw new InsufficientBalanceException("Não é possível estornar essa compra: a quantidade já foi vendida!");

        transaction.Reverse();
    }

    public void ReverseDividend(Guid dividendId)
    {
        var dividend = _dividends.FirstOrDefault(x => x.Id == dividendId)
            ?? throw new DividendNotFoundException();

        dividend.Reverse();
    }
}
