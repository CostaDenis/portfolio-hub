using System.Net.Http.Json;
using PortfolioHub.Web.Api.Abstractions;
using PortfolioHub.Web.Api.Models;
using PortfolioHub.Web.Api.Models.Asset.Requests;
using PortfolioHub.Web.Api.Models.Asset.Responses;

namespace PortfolioHub.Web.Api;

public class AssetApiClient(IHttpClientFactory httpClientFactory)
    : BaseApiClient(httpClientFactory), IAssetApiCient
{

    public async Task<IReadOnlyList<AssetResponse>> GetAssetsAsync(CancellationToken cancellationToken = default)
    => await _client.GetFromJsonAsync<List<AssetResponse>>("v1/assets", cancellationToken) ?? [];

    public async Task<ApiResult<AssetResponse>> CreateAssetAsync(CreateAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("v1/assets", request, cancellationToken);
        return await ReadResultAsync<AssetResponse>(response, cancellationToken);
    }

    public async Task<ApiResult> UpdateAssetAsync(Guid assetId, UpdateAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PutAsJsonAsync($"v1/assets/{assetId}", request, cancellationToken);
        return await ReadResultAsync(response, cancellationToken);
    }

    public async Task<ApiResult> UpdateMarketPriceAsync(Guid assetId, decimal price,
        CancellationToken cancellationToken = default)
    {
        using var response = await _client.PutAsJsonAsync(
            $"v1/assets/{assetId}/market-price", new UpdateMarketPriceRequest(price), cancellationToken);
        return await ReadResultAsync(response, cancellationToken);
    }
}
