namespace PortfolioHub.Web.Api.Models.Wallet.Requests;

public record ReceiveDividendRequest(Guid AssetId, decimal ValuePerShare, DateTime Date);