namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record WalletSummaryResponse(Guid WalletId,
    string Name, decimal NetWorth, int AssetCount);