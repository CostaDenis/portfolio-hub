using PortfolioHub.Application.DTOs;
using PortfolioHub.Application.Queries.Wallets;
using PortfolioHub.Application.Services;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Handlers.Queries.Wallets;

public class GetWalletTransactionsQueryHandler(WalletFinder walletFinder)
{

    public async Task<IReadOnlyCollection<TransactionDTO>> HandleAsync(GetWalletTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.StartDate.HasValue && query.EndDate.HasValue
            && query.StartDate > query.EndDate)
            throw new ArgumentException("A data inicial não pode ser maior que a data final!");

        if (query.Type.HasValue && !Enum.IsDefined(query.Type.Value))
            throw new ArgumentOutOfRangeException(nameof(query.Type));

        var wallet = await walletFinder.GetRequiredAsync(query.WalletId, cancellationToken);

        IEnumerable<Transaction> transactions = wallet.Transactions;

        if (query.AssetId.HasValue)
            transactions = transactions.Where(transaction => transaction.Asset.Id == query.AssetId.Value);

        if (query.Type.HasValue)
            transactions = transactions.Where(transaction => transaction.Type == query.Type.Value);

        if (query.StartDate.HasValue)
            transactions = transactions.Where(transaction => transaction.Date >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            transactions = transactions.Where(transaction => transaction.Date <= query.EndDate.Value);

        return transactions
            .Select(transaction => new TransactionDTO(
                transaction.Id,
                transaction.Asset.Id,
                transaction.Asset.Ticker,
                transaction.Type,
                transaction.Date,
                transaction.Quantity,
                transaction.UnitPrice,
                transaction.Total))
            .ToList()
            .AsReadOnly();
    }
}
