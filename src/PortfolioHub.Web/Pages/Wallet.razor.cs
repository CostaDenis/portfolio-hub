using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PortfolioHub.Web.Api;
using PortfolioHub.Web.Api.Abstractions;
using PortfolioHub.Web.Api.Models.Asset.Responses;
using PortfolioHub.Web.Api.Models.Wallet.Requests;
using PortfolioHub.Web.Api.Models.Wallet.Responses;
using PortfolioHub.Web.Components;

namespace PortfolioHub.Web.Pages;

public class WalletPage : ComponentBase, IDisposable
{
    [Inject] private IWalletApiClient WalletApi { get; set; } = default!;
    [Inject] private IAssetApiClient AssetApi { get; set; } = default!;

    [Parameter] public Guid WalletId { get; set; }

    protected const int RecentItems = 5;

    protected enum LoadState { Loading, Loaded, WalletNotFound, Error }

    protected enum OperationType { Buy, Sell, Dividend }

    protected static readonly IReadOnlyList<(OperationType Type, string Label)> OperationTypes =
    [
        (OperationType.Buy, "Compra"),
        (OperationType.Sell, "Venda"),
        (OperationType.Dividend, "Dividendo"),
    ];

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

    protected IReadOnlyList<AssetResponse> assets = [];
    private Dictionary<Guid, string> assetTypes = [];

    protected string? statusMessage;

    protected bool renaming;
    protected string newWalletName = "";
    protected string? renameError;
    protected bool savingName;
    protected ElementReference renameInput;
    protected ElementReference renameButton;

    private enum FocusTarget { None, RenameInput, RenameButton }

    private FocusTarget pendingFocus;

    protected bool operationOpen;
    protected OperationType operationType = OperationType.Buy;
    protected Guid? operationAssetId;
    protected decimal? operationQuantity;
    protected decimal? operationPrice;
    protected DateTime? operationDate;
    protected string? operationError;
    protected bool savingOperation;

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

    // O campo e o botão só existem depois do render, então o foco é aplicado aqui.
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var target = pendingFocus;
        pendingFocus = FocusTarget.None;

        if (target == FocusTarget.RenameInput)
            await renameInput.FocusAsync();
        else if (target == FocusTarget.RenameButton)
            await renameButton.FocusAsync();
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

            await FetchWalletDataAsync();
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

    // Busca posições, movimentações, dividendos e ativos sem trocar a tela para "carregando".
    private async Task FetchWalletDataAsync()
    {
        var positionsTask = WalletApi.GetPositionsAsync(WalletId, cts.Token);
        var transactionsTask = WalletApi.GetTransactionsAsync(WalletId, cts.Token);
        var dividendsTask = WalletApi.GetDividendsAsync(WalletId, cts.Token);
        var assetsTask = AssetApi.GetAssetsAsync(cts.Token);

        await Task.WhenAll(positionsTask, transactionsTask, dividendsTask, assetsTask);

        positions = positionsTask.Result.OrderByDescending(p => p.Total).ToList();
        transactions = transactionsTask.Result;
        dividends = dividendsTask.Result;
        assets = assetsTask.Result.OrderBy(a => a.Ticker).ToList();
        assetTypes = assets.ToDictionary(a => a.AssetId, a => a.Type);
        allocation = BuildAllocation();
    }

    protected void OpenRename()
    {
        renaming = true;
        newWalletName = wallet!.Name;
        renameError = null;
        statusMessage = null;
        pendingFocus = FocusTarget.RenameInput;
    }

    protected void CloseRename()
    {
        renaming = false;
        renameError = null;
        pendingFocus = FocusTarget.RenameButton;
    }

    protected void OnRenameKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape")
            CloseRename();
    }

    protected async Task SaveNameAsync()
    {
        if (savingName)
            return;

        var name = newWalletName.Trim();

        if (name == wallet!.Name)
        {
            CloseRename();
            return;
        }

        savingName = true;
        renameError = null;

        try
        {
            var result = await WalletApi.UpdateWalletNameAsync(WalletId, new UpdateWalletNameRequest(name), cts.Token);

            if (!result.Succeeded)
            {
                renameError = result.Error;
                return;
            }

            wallet = result.Value!;
            statusMessage = $"Carteira renomeada para \"{wallet.Name}\".";
            CloseRename();
        }
        catch (HttpRequestException)
        {
            renameError = "Não foi possível falar com a API. Tente novamente.";
        }
        finally
        {
            savingName = false;
        }
    }

    // Compra: qualquer ativo cadastrado. Venda e dividendo: só ativos que a carteira possui.
    protected IReadOnlyList<(Guid AssetId, string Label)> OperationAssetOptions
        => operationType == OperationType.Buy
            ? assets.Select(a => (a.AssetId, $"{a.Ticker} — {a.Name}")).ToList()
            : positions.OrderBy(p => p.Ticker)
                .Select(p => (p.AssetId, $"{p.Ticker} — {FormatQuantity(p.Quantity)} em carteira")).ToList();

    protected decimal HeldQuantity
        => positions.FirstOrDefault(p => p.AssetId == operationAssetId)?.Quantity ?? 0;

    protected decimal? OperationTotal => operationType switch
    {
        OperationType.Dividend when operationPrice is not null && operationAssetId is not null => HeldQuantity * operationPrice,
        OperationType.Buy or OperationType.Sell when operationPrice is not null && operationQuantity is not null => operationQuantity * operationPrice,
        _ => null,
    };

    protected string SubmitLabel => savingOperation ? "Registrando…" : operationType switch
    {
        OperationType.Buy => "Registrar compra",
        OperationType.Sell => "Registrar venda",
        _ => "Registrar dividendo",
    };

    protected void OpenOperation()
    {
        operationOpen = true;
        statusMessage = null;
        SetOperationType(OperationType.Buy);
    }

    protected void CloseOperation()
    {
        operationOpen = false;
        operationError = null;
    }

    protected void SetOperationType(OperationType type)
    {
        operationType = type;
        operationAssetId = null;
        operationQuantity = null;
        operationPrice = null;
        operationDate = type == OperationType.Dividend ? DateTime.Today : null;
        operationError = null;
    }

    protected async Task SubmitOperationAsync()
    {
        if (savingOperation)
            return;

        operationError = ValidateOperation();
        if (operationError is not null)
            return;

        savingOperation = true;

        try
        {
            var assetId = operationAssetId!.Value;
            var ticker = assets.FirstOrDefault(a => a.AssetId == assetId)?.Ticker ?? "ativo";
            var total = OperationTotal;

            var result = operationType switch
            {
                OperationType.Buy => await WalletApi.BuyAssetAsync(WalletId,
                    new TradeAssetRequest(assetId, operationQuantity!.Value, operationPrice!.Value), cts.Token),
                OperationType.Sell => await WalletApi.SellAssetAsync(WalletId,
                    new TradeAssetRequest(assetId, operationQuantity!.Value, operationPrice!.Value), cts.Token),
                _ => await WalletApi.ReceiveDividendAsync(WalletId,
                    new ReceiveDividendRequest(assetId, operationPrice!.Value, operationDate!.Value), cts.Token),
            };

            if (!result.Succeeded)
            {
                operationError = result.Error;
                return;
            }

            statusMessage = operationType switch
            {
                OperationType.Buy => $"Compra de {FormatQuantity(operationQuantity!.Value)} {ticker} registrada.",
                OperationType.Sell => $"Venda de {FormatQuantity(operationQuantity!.Value)} {ticker} registrada.",
                _ => $"Dividendo de {ticker} registrado: {total?.ToString("C")}.",
            };

            CloseOperation();
            await FetchWalletDataAsync();
        }
        catch (HttpRequestException)
        {
            operationError = "Não foi possível falar com a API. Tente novamente.";
        }
        finally
        {
            savingOperation = false;
        }
    }

    private string? ValidateOperation()
    {
        if (operationAssetId is null)
            return "Selecione um ativo.";

        if (operationType != OperationType.Dividend && operationQuantity is not > 0)
            return "Informe uma quantidade maior que zero.";

        if (operationPrice is not > 0)
            return operationType == OperationType.Dividend
                ? "Informe o valor por cota."
                : "Informe um preço unitário maior que zero.";

        if (operationType == OperationType.Dividend && operationDate is null)
            return "Informe a data do pagamento.";

        return null;
    }

    // Blazor omite atributos bool falsos; atributos ARIA precisam do texto "true"/"false".
    protected static string AriaBool(bool value) => value ? "true" : "false";

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
