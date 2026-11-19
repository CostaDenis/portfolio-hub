using System.Globalization;
using System.Net.Http.Json;
using PortfolioHub.Application.Providers;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Infrastructure.Providers.Models;

namespace PortfolioHub.Infrastructure.Providers;

public class BinancePriceProvider(HttpClient client) : IMarketPriceProvider
{

    private const string QuoteCurrency = "BRL";

    public bool Supports(EAssetType type) => type == EAssetType.Cryptocurrency;

    public async Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync(IReadOnlyCollection<string> tickers, CancellationToken cancellationToken)
    {
        var allPrices = await client.GetFromJsonAsync<List<BinanceTickerPrice>>(
            "api/v3/ticker/price", cancellationToken) ?? [];
        var pricesBySymbol = allPrices
            .ToDictionary(x => x.Symbol, x => x.Price);

        var result = new Dictionary<string, decimal>();

        foreach (var ticker in tickers)
        {
            if (!pricesBySymbol.TryGetValue(ticker + QuoteCurrency, out var price))
                continue;

            var value = decimal.Parse(price, CultureInfo.InvariantCulture);

            if (value <= 0)
                continue;

            result[ticker] = value;
        }

        return result;
    }
}