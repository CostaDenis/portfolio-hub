namespace PortfolioHub.Web.Api.Models.Wallet.Responses;

public record WalletDividendsResponse
    (decimal TotalReceived, IReadOnlyList<DividendResponse> Dividends);