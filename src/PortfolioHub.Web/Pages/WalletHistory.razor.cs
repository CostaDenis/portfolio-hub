using System.Globalization;
using Microsoft.AspNetCore.Components;
using PortfolioHub.Web.Api.Abstractions;
using PortfolioHub.Web.Api.Models.Wallet.Responses;

namespace PortfolioHub.Web.Pages;

public class WalletHistoryPage : ComponentBase, IDisposable
{
    [Inject] private IWalletApiClient WalletApi { get; set; } = default!;

    [Parameter] public Guid WalletId { get; set; }

    // Permite abrir já filtrado a partir do dashboard: ?type=dividends
    [SupplyParameterFromQuery(Name = "type")] public string? KindQuery { get; set; }

    protected enum LoadState { Loading, Loaded, WalletNotFound, Error }

    protected enum EntryKind { Buy, Sell, Dividend }

    protected record HistoryEntry(EntryKind Kind, string Ticker, DateTime Date,
        decimal Quantity, decimal UnitPrice, decimal Total);

    protected record MonthGroup(string Label, IReadOnlyList<HistoryEntry> Entries);

    protected static readonly IReadOnlyList<(EntryKind Kind, string Label, string Query)> Kinds =
    [
        (EntryKind.Buy, "Compras", "buys"),
        (EntryKind.Sell, "Vendas", "sells"),
        (EntryKind.Dividend, "Dividendos", "dividends"),
    ];

    private readonly CancellationTokenSource cts = new();

    protected LoadState state = LoadState.Loading;
    protected WalletResponse? wallet;
    protected IReadOnlyList<HistoryEntry> entries = [];
    protected IReadOnlyList<string> tickers = [];

    protected EntryKind? kindFilter;
    protected string tickerFilter = "";

    protected IEnumerable<HistoryEntry> Filtered
        => entries.Where(e => (kindFilter is null || e.Kind == kindFilter)
                              && (tickerFilter.Length == 0 || e.Ticker == tickerFilter));

    protected IReadOnlyList<MonthGroup> Groups
        => Filtered
            .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .Select(g => new MonthGroup(MonthLabel(g.Key), g.ToList()))
            .ToList();

    protected decimal TotalBought => Filtered.Where(e => e.Kind == EntryKind.Buy).Sum(e => e.Total);
    protected decimal TotalSold => Filtered.Where(e => e.Kind == EntryKind.Sell).Sum(e => e.Total);
    protected decimal TotalDividends => Filtered.Where(e => e.Kind == EntryKind.Dividend).Sum(e => e.Total);

    protected bool HasFilters => kindFilter is not null || tickerFilter.Length > 0;

    protected override async Task OnParametersSetAsync()
    {
        var query = KindQuery?.ToLowerInvariant();
        kindFilter = Kinds.Where(k => k.Query == query).Select(k => (EntryKind?)k.Kind).FirstOrDefault();

        await LoadAsync();
    }

    protected async Task LoadAsync()
    {
        state = LoadState.Loading;

        try
        {
            wallet = await WalletApi.GetWalletAsync(WalletId, cts.Token);

            if (wallet is null)
            {
                state = LoadState.WalletNotFound;
                return;
            }

            var transactionsTask = WalletApi.GetTransactionsAsync(WalletId, cts.Token);
            var dividendsTask = WalletApi.GetDividendsAsync(WalletId, cts.Token);

            await Task.WhenAll(transactionsTask, dividendsTask);

            entries = BuildEntries(transactionsTask.Result, dividendsTask.Result.Dividends);
            tickers = entries.Select(e => e.Ticker).Distinct().Order().ToList();
            state = LoadState.Loaded;
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Página fechada durante o carregamento.
        }
        catch (Exception exception) when (exception is HttpRequestException or System.Text.Json.JsonException)
        {
            state = LoadState.Error;
        }
    }

    // A API devolve a data da transação em UTC; o dividendo já vem com a data informada pelo usuário.
    private static List<HistoryEntry> BuildEntries(
        IEnumerable<TransactionResponse> transactions, IEnumerable<DividendResponse> dividends)
        => transactions
            .Select(t => new HistoryEntry(t.IsBuy ? EntryKind.Buy : EntryKind.Sell, t.Ticker,
                t.Date.ToLocalTime(), t.Quantity, t.UnitPrice, t.Total))
            .Concat(dividends.Select(d => new HistoryEntry(EntryKind.Dividend, d.Ticker,
                d.Date, d.Quantity, d.ValuePerShare, d.Total)))
            .OrderByDescending(e => e.Date)
            .ToList();

    protected void SetKindFilter(EntryKind? kind) => kindFilter = kind;

    protected void ClearFilters()
    {
        kindFilter = null;
        tickerFilter = "";
    }

    protected int CountByKind(EntryKind? kind)
        => entries.Count(e => (kind is null || e.Kind == kind)
                              && (tickerFilter.Length == 0 || e.Ticker == tickerFilter));

    protected static string KindLabel(EntryKind kind) => kind switch
    {
        EntryKind.Buy => "Compra",
        EntryKind.Sell => "Venda",
        _ => "Dividendo",
    };

    protected static string BadgeClass(EntryKind kind) => kind switch
    {
        EntryKind.Buy => "bg-primary/10 text-primary",
        EntryKind.Sell => "bg-link/10 text-link",
        _ => "bg-success/10 text-success",
    };

    private static string MonthLabel(DateTime month)
    {
        var label = month.ToString("MMMM 'de' yyyy");
        return CultureInfo.CurrentCulture.TextInfo.ToUpper(label[0]) + label[1..];
    }

    // Blazor omite atributos bool falsos; atributos ARIA precisam do texto "true"/"false".
    protected static string AriaBool(bool value) => value ? "true" : "false";

    protected static string ChipClass(bool active)
        => "hover:cursor-pointer inline-flex items-center gap-1.5 rounded-full border px-3 py-1.5 text-xs font-medium transition-colors focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-link "
           + (active ? "border-ink bg-ink text-surface" : "border-line bg-surface text-muted hover:border-muted hover:text-ink");

    protected static string FormatQuantity(decimal quantity) => quantity.ToString("#,##0.########");

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
