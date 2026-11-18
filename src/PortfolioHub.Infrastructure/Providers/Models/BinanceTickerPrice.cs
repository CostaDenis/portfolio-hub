namespace PortfolioHub.Infrastructure.Providers.Models;

//o preco vem como string pela API da Binance
public record class BinanceTickerPrice(string Symbol, string Price);