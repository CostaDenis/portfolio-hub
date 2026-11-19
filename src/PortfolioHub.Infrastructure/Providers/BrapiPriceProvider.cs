using System.Net;
using System.Net.Http.Json;
using PortfolioHub.Application.Providers;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Infrastructure.Providers.Models;

namespace PortfolioHub.Infrastructure.Providers;

public class BrapiPriceProvider(HttpClient client) : IMarketPriceProvider
{

    public bool Supports(EAssetType type)
        => type is EAssetType.Stock or EAssetType.RealStateFund;

    public async Task<IReadOnlyDictionary<string, decimal>> GetPricesAsync(IReadOnlyCollection<string> tickers, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, decimal>();

        //um request por vez(limite do plano gratuito)
        foreach (var ticker in tickers)
        {
            using var response = await client
                .GetAsync($"api/quote/{ticker}", cancellationToken);

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
                continue;

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<BrapiQuoteResponse>(cancellationToken);
            var price = body?.Results.FirstOrDefault()?.RegularMarketPrice ?? 0;

            if (price <= 0)
                continue;

            result[ticker] = price;
        }

        return result;
    }

}