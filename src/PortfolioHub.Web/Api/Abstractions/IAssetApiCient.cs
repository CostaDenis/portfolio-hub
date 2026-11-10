using PortfolioHub.Web.Api.Models;
using PortfolioHub.Web.Api.Models.Asset.Requests;
using PortfolioHub.Web.Api.Models.Asset.Responses;

namespace PortfolioHub.Web.Api.Abstractions;

public interface IAssetApiCient
{
    Task<IReadOnlyList<AssetResponse>> GetAssetsAsync(CancellationToken cancellationToken = default);

    Task<ApiResult<AssetResponse>> CreateAssetAsync(CreateAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> UpdateAssetAsync(Guid assetId, UpdateAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> UpdateMarketPriceAsync(Guid assetId, decimal price,
        CancellationToken cancellationToken = default);
}
