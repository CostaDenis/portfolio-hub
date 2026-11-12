using Microsoft.AspNetCore.Components;
using PortfolioHub.Web.Api.Abstractions;
using PortfolioHub.Web.Api.Models.Wallet.Requests;
using PortfolioHub.Web.Api.Models.Wallet.Responses;

namespace PortfolioHub.Web.Pages;

public class HomePage : ComponentBase, IDisposable
{
    [Inject] private IWalletApiClient WalletApi { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected enum LoadState { Loading, Loaded, Error }

    private readonly CancellationTokenSource cts = new();

    protected LoadState state = LoadState.Loading;
    protected IReadOnlyList<WalletSummaryResponse> wallets = [];

    protected bool showForm;
    protected bool creating;
    protected string newWalletName = "";
    protected string? createError;
    protected ElementReference nameInput;

    private bool focusInput;

    protected decimal TotalNetWorth => wallets.Sum(wallet => wallet.NetWorth);

    protected override async Task OnInitializedAsync()
        => await LoadAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!focusInput)
            return;

        focusInput = false;
        await nameInput.FocusAsync();
    }

    protected async Task LoadAsync()
    {
        state = LoadState.Loading;

        try
        {
            wallets = await WalletApi.GetWalletsAsync(cts.Token);
            showForm = wallets.Count == 0;
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

    protected void OpenForm()
    {
        showForm = true;
        focusInput = true;
    }

    protected void CloseForm()
    {
        showForm = false;
        newWalletName = "";
        createError = null;
    }

    protected async Task CreateWalletAsync()
    {
        if (creating)
            return;

        creating = true;
        createError = null;

        try
        {
            var result = await WalletApi.CreateWalletAsync
                (new CreateWalletRequest(newWalletName.Trim()), cts.Token);

            if (!result.Succeeded)
            {
                createError = result.Error;
                return;
            }

            Navigation.NavigateTo($"wallets/{result.Value!.WalletId}");
        }
        catch (HttpRequestException)
        {
            createError = "Não foi possível falar com a API. Tente novamente.";
        }
        finally
        {
            creating = false;
        }
    }

    protected static string AssetCountLabel(int count) => count switch
    {
        0 => "Nenhum ativo",
        1 => "1 ativo",
        _ => $"{count} ativos",
    };

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
