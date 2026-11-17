using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    protected record HistoryEntry(Guid Id, EntryKind Kind, string Ticker, DateTime Date,
        decimal Quantity, decimal UnitPrice, decimal Total, bool IsReversed);

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

    protected string? statusMessage;

    protected Guid? confirmingId;
    protected string? reverseError;
    protected bool reversing;
    protected ElementReference confirmButton;
    protected ElementReference statusElement;
    protected readonly Dictionary<Guid, ElementReference> reverseButtons = [];

    private enum FocusTarget { None, ConfirmButton, ReverseButton, Status }

    private FocusTarget pendingFocus;
    private Guid pendingFocusId;

    protected IEnumerable<HistoryEntry> Filtered
        => entries.Where(e => (kindFilter is null || e.Kind == kindFilter)
                              && (tickerFilter.Length == 0 || e.Ticker == tickerFilter));

    protected IReadOnlyList<MonthGroup> Groups
        => Filtered
            .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .Select(g => new MonthGroup(MonthLabel(g.Key), g.ToList()))
            .ToList();

    // Estornados continuam na lista, mas não entram nos totais.
    protected decimal TotalBought => SumActive(EntryKind.Buy);
    protected decimal TotalSold => SumActive(EntryKind.Sell);
    protected decimal TotalDividends => SumActive(EntryKind.Dividend);

    private decimal SumActive(EntryKind kind)
        => Filtered.Where(e => e.Kind == kind && !e.IsReversed).Sum(e => e.Total);

    protected bool HasFilters => kindFilter is not null || tickerFilter.Length > 0;

    protected override async Task OnParametersSetAsync()
    {
        var query = KindQuery?.ToLowerInvariant();
        kindFilter = Kinds.Where(k => k.Query == query).Select(k => (EntryKind?)k.Kind).FirstOrDefault();

        await LoadAsync();
    }

    // Os botões só existem depois do render, então o foco é aplicado aqui.
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var target = pendingFocus;
        pendingFocus = FocusTarget.None;

        if (target == FocusTarget.ConfirmButton)
            await confirmButton.FocusAsync();
        else if (target == FocusTarget.Status)
            await statusElement.FocusAsync();
        else if (target == FocusTarget.ReverseButton && reverseButtons.TryGetValue(pendingFocusId, out var button))
            await button.FocusAsync();
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

            await FetchEntriesAsync();
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

    // Busca as movimentações sem trocar a tela para "carregando".
    private async Task FetchEntriesAsync()
    {
        var transactionsTask = WalletApi.GetTransactionsAsync(WalletId, cts.Token);
        var dividendsTask = WalletApi.GetDividendsAsync(WalletId, cts.Token);

        await Task.WhenAll(transactionsTask, dividendsTask);

        entries = BuildEntries(transactionsTask.Result, dividendsTask.Result.Dividends);
        tickers = entries.Select(e => e.Ticker).Distinct().Order().ToList();
    }

    // A API devolve a data da transação em UTC; o dividendo já vem com a data informada pelo usuário.
    private static List<HistoryEntry> BuildEntries(
        IEnumerable<TransactionResponse> transactions, IEnumerable<DividendResponse> dividends)
        => transactions
            .Select(t => new HistoryEntry(t.TransactionId, t.IsBuy ? EntryKind.Buy : EntryKind.Sell, t.Ticker,
                t.Date.ToLocalTime(), t.Quantity, t.UnitPrice, t.Total, t.IsReversed))
            .Concat(dividends.Select(d => new HistoryEntry(d.DividendId, EntryKind.Dividend, d.Ticker,
                d.Date, d.Quantity, d.ValuePerShare, d.Total, d.IsReversed)))
            .OrderByDescending(e => e.Date)
            .ToList();

    protected void SetKindFilter(EntryKind? kind)
    {
        kindFilter = kind;
        confirmingId = null;
    }

    protected void ClearFilters()
    {
        kindFilter = null;
        tickerFilter = "";
        confirmingId = null;
    }

    protected void OnTickerChanged() => confirmingId = null;

    protected void OpenReverse(HistoryEntry entry)
    {
        confirmingId = entry.Id;
        reverseError = null;
        statusMessage = null;
        pendingFocus = FocusTarget.ConfirmButton;
    }

    protected void CancelReverse()
    {
        if (confirmingId is { } id)
        {
            pendingFocus = FocusTarget.ReverseButton;
            pendingFocusId = id;
        }

        confirmingId = null;
        reverseError = null;
    }

    protected void OnConfirmKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && !reversing)
            CancelReverse();
    }

    protected async Task ConfirmReverseAsync(HistoryEntry entry)
    {
        if (reversing)
            return;

        reversing = true;
        reverseError = null;

        try
        {
            var result = entry.Kind == EntryKind.Dividend
                ? await WalletApi.ReverseDividendAsync(WalletId, entry.Id, cts.Token)
                : await WalletApi.ReverseTransactionAsync(WalletId, entry.Id, cts.Token);

            if (!result.Succeeded)
            {
                reverseError = result.Error;
                return;
            }

            confirmingId = null;
            statusMessage = $"{KindLabel(entry.Kind)} de {entry.Ticker} em {entry.Date:dd/MM/yyyy} {ReversedLabel(entry.Kind).ToLowerInvariant()}.";
            pendingFocus = FocusTarget.Status;

            await FetchEntriesAsync();
        }
        catch (HttpRequestException)
        {
            reverseError = "Não foi possível falar com a API. Tente novamente.";
        }
        finally
        {
            reversing = false;
        }
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

    protected static string ReversedLabel(EntryKind kind) => kind == EntryKind.Dividend ? "Estornado" : "Estornada";

    protected static string ConfirmTitle(EntryKind kind) => kind switch
    {
        EntryKind.Buy => "Estornar esta compra?",
        EntryKind.Sell => "Estornar esta venda?",
        _ => "Estornar este dividendo?",
    };

    protected static string ConfirmDetail(HistoryEntry entry) => entry.Kind switch
    {
        EntryKind.Buy => $"{FormatQuantity(entry.Quantity)} {entry.Ticker} saem da carteira e o valor deixa de contar como investido.",
        EntryKind.Sell => $"{FormatQuantity(entry.Quantity)} {entry.Ticker} voltam para a carteira.",
        _ => $"{entry.Total:C} deixam de contar nos dividendos recebidos.",
    };

    protected static string BadgeClass(HistoryEntry entry) => entry.IsReversed
        ? "bg-line text-muted"
        : entry.Kind switch
        {
            EntryKind.Buy => "bg-primary/10 text-primary",
            EntryKind.Sell => "bg-link/10 text-link",
            _ => "bg-success/10 text-success",
        };

    protected static string TotalClass(HistoryEntry entry)
        => entry.IsReversed ? "text-muted line-through"
            : entry.Kind == EntryKind.Dividend ? "text-success" : "";

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
