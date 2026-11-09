using Microsoft.AspNetCore.Components;
using PortfolioHub.Web.Components;
using PortfolioHub.Web.Models;
using PortfolioHub.Web.Services;

namespace PortfolioHub.Web.Pages;

public class WalletPage : ComponentBase, IDisposable
{
    [Inject] private PortfolioApiClient Api { get; set; } = default!;

    [Parameter] public Guid WalletId { get; set; }

    protected const int RecentItems = 5;

    protected enum LoadState { Loading, Loaded, WalletNotFound, Error }

    protected record AssetClass(string Label, string ColorClass);

    private static readonly Dictionary<string, AssetClass> AssetClasses = new()
    {
        ["Stock"] = new("Ações", "bg-series-1"),
        ["RealStateFund"] = new("Fundos imobiliários", "bg-series-2"),
        ["Cryptocurrency"] = new("Criptomoedas", "bg-series-3"),
    };

    private static readonly AssetClass UnknownClass = new("Outros", "bg-muted");

    private readonly CancellationTokenSource cts = new();

    protected LoadState state = LoadState.Loading;

    protected WalletResponse? wallet;
    protected IReadOnlyList<WalletPositionResponse> positions = [];
    protected IReadOnlyList<TransactionResponse> transactions = [];
    protected WalletDividendsResponse dividends = new(0, []);
    protected IReadOnlyList<AllocationBar.AllocationSlice> allocation = [];

    private Dictionary<Guid, string> assetTypes = [];

    protected decimal NetWorth => positions.Sum(p => p.Total);
    protected decimal NetInvested => transactions.Sum(t => t.IsBuy ? t.Total : -t.Total);

    // Patrimônio atual + o que já foi resgatado em vendas − o que foi gasto em compras.
    protected decimal Result => NetWorth - NetInvested;

    protected string AssetCountLabel => positions.Count == 1 ? "1 ativo em carteira" : $"{positions.Count} ativos em carteira";

    protected string DividendCountLabel => dividends.Dividends.Count switch
    {
        0 => "Nenhum pagamento ainda",
        1 => "1 pagamento",
        var n => $"{n} pagamentos",
    };

    // Para recarregar quando a rota troca de carteira.
    protected override async Task OnParametersSetAsync()
        => await LoadAsync();

    protected async Task LoadAsync()
    {
        state = LoadState.Loading;

        try
        {
            wallet = await Api.GetWalletAsync(WalletId, cts.Token);

            if (wallet is null)
            {
                state = LoadState.WalletNotFound;
                return;
            }

            var positionsTask = Api.GetPositionsAsync(WalletId, cts.Token);
            var transactionsTask = Api.GetTransactionsAsync(WalletId, cts.Token);
            var dividendsTask = Api.GetDividendsAsync(WalletId, cts.Token);
            var assetsTask = Api.GetAssetsAsync(cts.Token);

            await Task.WhenAll(positionsTask, transactionsTask, dividendsTask, assetsTask);

            positions = positionsTask.Result.OrderByDescending(p => p.Total).ToList();
            transactions = transactionsTask.Result;
            dividends = dividendsTask.Result;
            assetTypes = assetsTask.Result.ToDictionary(a => a.AssetId, a => a.Type);
            allocation = BuildAllocation();

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

    protected AssetClass ClassOf(Guid assetId)
        => assetTypes.TryGetValue(assetId, out var type) && AssetClasses.TryGetValue(type, out var assetClass)
            ? assetClass
            : UnknownClass;

    protected static string FormatQuantity(decimal quantity) => quantity.ToString("#,##0.########");

    protected static string FormatSigned(decimal value)
        => value switch
        {
            > 0 => "+" + value.ToString("C"),
            < 0 => "−" + Math.Abs(value).ToString("C"),
            _ => value.ToString("C"),
        };

    private IReadOnlyList<AllocationBar.AllocationSlice> BuildAllocation()
    {
        var total = NetWorth;
        if (total == 0)
            return [];

        return positions
            .GroupBy(p => ClassOf(p.AssetId))
            .Select(g => (Class: g.Key, Value: g.Sum(p => p.Total)))
            .OrderBy(g => ClassOrder(g.Class))
            .Select(g => new AllocationBar.AllocationSlice(g.Class.Label, g.Class.ColorClass, g.Value, g.Value / total))
            .ToList();
    }

    private static int ClassOrder(AssetClass assetClass)
    {
        var index = AssetClasses.Values.ToList().IndexOf(assetClass);
        return index < 0 ? int.MaxValue : index;
    }

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
