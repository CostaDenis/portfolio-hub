using Microsoft.AspNetCore.Components;
using PortfolioHub.Web.Models;
using PortfolioHub.Web.Services;

namespace PortfolioHub.Web.Pages;

public class AssetsPage : ComponentBase, IDisposable
{
    [Inject] private PortfolioApiClient Api { get; set; } = default!;

    protected enum LoadState { Loading, Loaded, Error }

    protected enum FormMode { Closed, Create, Edit }

    protected record AssetTypeOption(string Value, string Label, string ColorClass);

    protected static readonly IReadOnlyList<AssetTypeOption> AssetTypes =
    [
        new("Stock", "Ações", "bg-series-1"),
        new("RealStateFund", "Fundos imobiliários", "bg-series-2"),
        new("Cryptocurrency", "Criptomoedas", "bg-series-3"),
    ];

    private readonly CancellationTokenSource cts = new();

    protected LoadState state = LoadState.Loading;
    protected List<AssetResponse> assets = [];

    protected string search = "";
    protected string? typeFilter;
    protected string? statusMessage;

    protected FormMode formMode = FormMode.Closed;
    protected string formName = "";
    protected string formTicker = "";
    protected string formType = "Stock";
    protected decimal? formPrice;
    protected string? formError;
    protected bool saving;
    protected string editingTicker = "";
    private Guid? editingAssetId;

    protected Guid? editingPriceAssetId;
    protected decimal? priceDraft;
    protected string? priceError;
    protected bool savingPrice;

    protected IReadOnlyList<AssetResponse> FilteredAssets => assets
        .Where(asset => typeFilter is null || asset.Type == typeFilter)
        .Where(asset => string.IsNullOrWhiteSpace(search)
            || asset.Ticker.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
            || asset.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
        .OrderBy(asset => asset.Ticker)
        .ToList();

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    protected async Task LoadAsync()
    {
        state = LoadState.Loading;

        try
        {
            assets = (await Api.GetAssetsAsync(cts.Token)).ToList();
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

    protected static AssetTypeOption TypeOf(string type)
        => AssetTypes.FirstOrDefault(option => option.Value == type) ?? new(type, type, "bg-muted");

    protected int CountByType(string? type)
        => type is null ? assets.Count : assets.Count(asset => asset.Type == type);

    protected void SetTypeFilter(string? type) => typeFilter = type;

    protected void OpenCreateForm()
    {
        CancelPriceEdit();
        formMode = FormMode.Create;
        editingAssetId = null;
        formName = formTicker = "";
        formType = "Stock";
        formPrice = null;
        formError = statusMessage = null;
    }

    protected void OpenEditForm(AssetResponse asset)
    {
        CancelPriceEdit();
        formMode = FormMode.Edit;
        editingAssetId = asset.AssetId;
        editingTicker = asset.Ticker;
        formName = asset.Name;
        formTicker = asset.Ticker;
        formType = asset.Type;
        formPrice = null;
        formError = statusMessage = null;
    }

    protected void CloseForm()
    {
        formMode = FormMode.Closed;
        editingAssetId = null;
        formError = null;
    }

    protected async Task SubmitForm()
    {
        if (saving)
            return;

        if (formMode == FormMode.Create && formPrice is null)
        {
            formError = "Informe o preço de mercado.";
            return;
        }

        saving = true;
        formError = null;

        try
        {
            if (formMode == FormMode.Create)
                await CreateAssetAsync();
            else
                await UpdateAssetAsync();
        }
        catch (HttpRequestException)
        {
            formError = "Não foi possível falar com a API. Tente novamente.";
        }
        finally
        {
            saving = false;
        }
    }

    private async Task CreateAssetAsync()
    {
        var request = new CreateAssetRequest(formName.Trim(), formTicker.Trim(), formType, formPrice!.Value);
        var result = await Api.CreateAssetAsync(request, cts.Token);

        if (!result.Succeeded)
        {
            formError = result.Error;
            return;
        }

        assets.Add(result.Value!);
        statusMessage = $"{result.Value!.Ticker} cadastrado com sucesso.";
        CloseForm();
    }

    private async Task UpdateAssetAsync()
    {
        var assetId = editingAssetId!.Value;
        var request = new UpdateAssetRequest(formName.Trim(), formTicker.Trim(), formType);
        var result = await Api.UpdateAssetAsync(assetId, request, cts.Token);

        if (!result.Succeeded)
        {
            formError = result.Error;
            return;
        }

        var updated = ReplaceAsset(assetId, asset => asset with
        {
            Name = request.AssetName,
            Ticker = request.Ticker.ToUpperInvariant(),
            Type = request.Type,
        });

        statusMessage = $"{updated?.Ticker} atualizado com sucesso.";
        CloseForm();
    }

    protected void StartPriceEdit(AssetResponse asset)
    {
        editingPriceAssetId = asset.AssetId;
        priceDraft = asset.Price;
        priceError = statusMessage = null;
    }

    protected void CancelPriceEdit()
    {
        editingPriceAssetId = null;
        priceError = null;
    }

    protected async Task SavePrice()
    {
        if (savingPrice || editingPriceAssetId is not { } assetId)
            return;

        if (priceDraft is null)
        {
            priceError = "Informe o novo preço.";
            return;
        }

        savingPrice = true;
        priceError = null;

        try
        {
            var result = await Api.UpdateMarketPriceAsync(assetId, priceDraft.Value, cts.Token);

            if (!result.Succeeded)
            {
                priceError = result.Error;
                return;
            }

            var updated = ReplaceAsset(assetId, asset => asset with { Price = priceDraft.Value });
            statusMessage = $"Preço de {updated?.Ticker} atualizado para {FormatPrice(priceDraft.Value)}.";
            CancelPriceEdit();
        }
        catch (HttpRequestException)
        {
            priceError = "Não foi possível falar com a API.";
        }
        finally
        {
            savingPrice = false;
        }
    }

    private AssetResponse? ReplaceAsset(Guid assetId, Func<AssetResponse, AssetResponse> update)
    {
        var index = assets.FindIndex(asset => asset.AssetId == assetId);
        if (index < 0)
            return null;

        assets[index] = update(assets[index]);
        return assets[index];
    }

    protected static string AriaBool(bool value) => value ? "true" : "false";

    protected static string ChipClass(bool active)
        => "hover:cursor-pointer inline-flex items-center gap-1.5 rounded-full border px-3 py-1.5 text-xs font-medium transition-colors focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-link "
           + (active ? "border-ink bg-ink text-surface" : "border-line bg-surface text-muted hover:border-muted hover:text-ink");

    protected static string FormatPrice(decimal price)
        => price < 1 ? price.ToString("C4") : price.ToString("C");

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
