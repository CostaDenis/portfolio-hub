using System.Net;
using System.Net.Http.Json;
using PortfolioHub.Web.Models;

namespace PortfolioHub.Web.Services;

public class PortfolioApiClient(HttpClient http)
{
    public Task<WalletResponse?> GetWalletAsync(Guid walletId, CancellationToken ct = default)
        => GetOrDefaultAsync<WalletResponse>($"v1/wallets/{walletId}", ct);

    public async Task<IReadOnlyList<WalletSummaryResponse>> GetWalletsAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<WalletSummaryResponse>>("v1/wallets", ct) ?? [];

    public async Task<ApiResult<WalletResponse>> CreateWalletAsync(string name, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("v1/wallets", new CreateWalletRequest(name), ct);
        return await ReadResultAsync<WalletResponse>(response, ct);
    }

    public async Task<IReadOnlyList<WalletPositionResponse>> GetPositionsAsync(Guid walletId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<WalletPositionResponse>>($"v1/wallets/{walletId}/position", ct) ?? [];

    public async Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync(Guid walletId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<TransactionResponse>>($"v1/wallets/{walletId}/transactions", ct) ?? [];

    public async Task<WalletDividendsResponse> GetDividendsAsync(Guid walletId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<WalletDividendsResponse>($"v1/wallets/{walletId}/dividends", ct)
            ?? new WalletDividendsResponse(0, []);

    public async Task<IReadOnlyList<AssetResponse>> GetAssetsAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<AssetResponse>>("v1/assets", ct) ?? [];

    public async Task<ApiResult<AssetResponse>> CreateAssetAsync(CreateAssetRequest request, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("v1/assets", request, ct);
        return await ReadResultAsync<AssetResponse>(response, ct);
    }

    public async Task<ApiResult> UpdateAssetAsync(Guid assetId, UpdateAssetRequest request, CancellationToken ct = default)
    {
        using var response = await http.PutAsJsonAsync($"v1/assets/{assetId}", request, ct);
        return await ReadResultAsync(response, ct);
    }

    public async Task<ApiResult> UpdateMarketPriceAsync(Guid assetId, decimal price, CancellationToken ct = default)
    {
        using var response = await http.PutAsJsonAsync($"v1/assets/{assetId}/market-price", new UpdateMarketPriceRequest(price), ct);
        return await ReadResultAsync(response, ct);
    }

    // Retorna null em 404 em vez de lançar exceção.
    private async Task<T?> GetOrDefaultAsync<T>(string uri, CancellationToken ct)
    {
        using var response = await http.GetAsync(uri, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ct);
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var error = await ReadErrorAsync(response, ct);
        if (error is not null)
            return ApiResult<T>.Fail(error);

        var value = await response.Content.ReadFromJsonAsync<T>(ct);
        return ApiResult<T>.Ok(value!);
    }

    private static async Task<ApiResult> ReadResultAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var error = await ReadErrorAsync(response, ct);
        return error is null ? ApiResult.Ok() : ApiResult.Fail(error);
    }

    // Erros de regra de negócio (400, 404, 409) viram mensagem para a tela;
    // qualquer outro erro lança HttpRequestException, tratado como falha de conexão.
    private static async Task<string?> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Conflict))
        {
            response.EnsureSuccessStatusCode();
            return null;
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(ct);
        return problem?.Detail ?? problem?.Title ?? "Não foi possível concluir a operação.";
    }
}

public record ApiResult(string? Error)
{
    public bool Succeeded => Error is null;
    public static ApiResult Ok() => new((string?)null);
    public static ApiResult Fail(string error) => new(error);
}

public record ApiResult<T>(T? Value, string? Error)
{
    public bool Succeeded => Error is null;
    public static ApiResult<T> Ok(T value) => new(value, null);
    public static ApiResult<T> Fail(string error) => new(default, error);
}

// Formato de erro padrão (ProblemDetails) devolvido pelo GlobalExceptionHandler da API.
public record ProblemResponse(string? Title, string? Detail);
