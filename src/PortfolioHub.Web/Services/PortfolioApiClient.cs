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

    // Em caso de dado inválido (400), devolve a mensagem de erro da API em vez de lançar exceção.
    public async Task<ApiResult<WalletResponse>> CreateWalletAsync(string name, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("v1/wallets", new CreateWalletRequest(name), ct);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(ct);
            return ApiResult<WalletResponse>.Fail(problem?.Detail ?? "Dados inválidos.");
        }

        response.EnsureSuccessStatusCode();
        var wallet = await response.Content.ReadFromJsonAsync<WalletResponse>(ct);
        return ApiResult<WalletResponse>.Ok(wallet!);
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

    // Retorna null em 404 em vez de lançar exceção.
    private async Task<T?> GetOrDefaultAsync<T>(string uri, CancellationToken ct)
    {
        using var response = await http.GetAsync(uri, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ct);
    }
}

public record ApiResult<T>(T? Value, string? Error)
{
    public bool Succeeded => Error is null;
    public static ApiResult<T> Ok(T value) => new(value, null);
    public static ApiResult<T> Fail(string error) => new(default, error);
}

// Formato de erro padrão (ProblemDetails) devolvido pelo GlobalExceptionHandler da API.
public record ProblemResponse(string? Title, string? Detail);
