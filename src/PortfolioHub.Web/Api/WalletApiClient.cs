using System.Net.Http.Json;
using PortfolioHub.Web.Api.Abstractions;
using PortfolioHub.Web.Api.Models;
using PortfolioHub.Web.Api.Models.Wallet.Requests;
using PortfolioHub.Web.Api.Models.Wallet.Responses;

namespace PortfolioHub.Web.Api;

public class WalletApiClient(HttpClient client)
    : BaseApiClient(client), IWalletApiClient
{
    public async Task<WalletDividendsResponse> GetDividendsAsync(Guid walletId,
        CancellationToken cancellationToken = default)
        => await _client.GetFromJsonAsync<WalletDividendsResponse>
            ($"v1/wallets/{walletId}/dividends", cancellationToken)
                ?? new WalletDividendsResponse(0, []);

    public async Task<IReadOnlyList<WalletPositionResponse>> GetPositionsAsync(Guid walletId,
        CancellationToken cancellationToken = default)
        => await _client.GetFromJsonAsync<List<WalletPositionResponse>>
            ($"v1/wallets/{walletId}/position", cancellationToken) ?? [];

    public async Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync(Guid walletId,
        CancellationToken cancellationToken = default)
        => await _client.GetFromJsonAsync<List<TransactionResponse>>
            ($"v1/wallets/{walletId}/transactions", cancellationToken) ?? [];

    public async Task<WalletResponse?> GetWalletAsync(Guid walletId,
        CancellationToken cancellationToken = default)
         => await GetOrDefaultAsync<WalletResponse>
            ($"v1/wallets/{walletId}", cancellationToken);

    public async Task<IReadOnlyList<WalletSummaryResponse>> GetWalletsAsync(CancellationToken cancellationToken = default)
         => await _client.GetFromJsonAsync<List<WalletSummaryResponse>>
            ("v1/wallets", cancellationToken) ?? [];

    public async Task<ApiResult<WalletResponse>> CreateWalletAsync(CreateWalletRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync
            ("v1/wallets", request, cancellationToken);

        return await ReadResultAsync<WalletResponse>(response, cancellationToken);
    }

    public async Task<ApiResult<WalletResponse>> UpdateWalletNameAsync(Guid walletId, UpdateWalletNameRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PutAsJsonAsync
            ($"v1/wallets/{walletId}", request, cancellationToken);

        return await ReadResultAsync<WalletResponse>(response, cancellationToken);
    }

    public async Task<ApiResult> BuyAssetAsync(Guid walletId, TradeAssetRequest request,
        CancellationToken cancellationToken = default)
        => await PostAsync($"v1/wallets/{walletId}/assets/buy", request, cancellationToken);

    public async Task<ApiResult> ReceiveDividendAsync(Guid walletId, ReceiveDividendRequest request,
        CancellationToken cancellationToken = default)
        => await PostAsync($"v1/wallets/{walletId}/assets/receive-dividend", request, cancellationToken);

    public async Task<ApiResult> SellAssetAsync(Guid walletId, TradeAssetRequest request,
        CancellationToken cancellationToken = default)
        => await PostAsync($"v1/wallets/{walletId}/assets/sell", request, cancellationToken);
}
