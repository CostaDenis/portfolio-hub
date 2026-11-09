namespace PortfolioHub.Web.Models;

// Espelham os DTOs da API (PortfolioHub.Application.DTOs).
// Ficam separados para o front não depender das camadas de backend.

public record WalletResponse(Guid WalletId, string Name);

public record WalletSummaryResponse(Guid WalletId, string Name, decimal NetWorth, int AssetCount);

public record CreateWalletRequest(string Name);

public record AssetResponse(Guid AssetId, string Name, string Ticker, string Type, decimal Price);

public record CreateAssetRequest(string AssetName, string Ticker, string Type, decimal MarketPrice);

public record UpdateAssetRequest(string AssetName, string Ticker, string Type);

public record UpdateMarketPriceRequest(decimal Price);

public record WalletPositionResponse(
    Guid AssetId, string AssetName, string Ticker, decimal Quantity, decimal MarketPrice)
{
    public decimal Total => Quantity * MarketPrice;
}

public record TransactionResponse(
    Guid TransactionId, Guid AssetId, string Ticker, string Type,
    DateTime Date, decimal Quantity, decimal UnitPrice, decimal Total)
{
    public bool IsBuy => Type == "Buy";
}

public record DividendResponse(
    Guid DividendId, Guid AssetId, string Ticker, decimal Quantity,
    decimal ValuePerShare, DateTime Date, decimal Total);

public record WalletDividendsResponse(decimal TotalReceived, IReadOnlyList<DividendResponse> Dividends);
