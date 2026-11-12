using PortfolioHub.Web.Api.Models;
using PortfolioHub.Web.Api.Models.Asset.Requests;
using PortfolioHub.Web.Api.Models.Wallet.Requests;
using PortfolioHub.Web.Api.Models.Wallet.Responses;

namespace PortfolioHub.Web.Api.Abstractions;

public interface IWalletApiClient
{
    Task<WalletResponse?> GetWalletAsync(Guid walletId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WalletSummaryResponse>> GetWalletsAsync(CancellationToken cancellationToken = default);

    Task<ApiResult<WalletResponse>> CreateWalletAsync(CreateWalletRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<WalletResponse>> UpdateWalletNameAsync(Guid walletId, UpdateWalletNameRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> BuyAssetAsync(Guid walletId, TradeAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> SellAssetAsync(Guid walletId, TradeAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> ReceiveDividendAsync(Guid walletId, ReceiveDividendRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WalletPositionResponse>> GetPositionsAsync(Guid walletId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionResponse>> GetTransactionsAsync(Guid walletId,
        CancellationToken cancellationToken = default);

    Task<WalletDividendsResponse> GetDividendsAsync(Guid walletId,
        CancellationToken cancellationToken = default);

}
