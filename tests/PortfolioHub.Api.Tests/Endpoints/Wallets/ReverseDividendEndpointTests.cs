using System.Net;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class ReverseDividendEndpointTests : ApiTestBase
{

    private readonly Wallet _wallet;
    private readonly Asset _bbas3;

    public ReverseDividendEndpointTests()
    {
        _wallet = new(new WalletName("Ações"));
        _bbas3 = new(new AssetName("Banco do Brasil"), new Ticker("BBAS3"),
            EAssetType.Stock, new MarketPrice(17m));

        _wallet.BuyAsset(_bbas3, 40, 16m);
    }

    [TestMethod]
    [TestCategory("ReverseDividendEndpoint Tests")]
    public async Task Should_Return_NoContent_And_Persist_Reversal_When_Dividend_Exists()
    {
        _wallet.ReceiveDividend(_bbas3, 0.68m, DateTime.UtcNow);
        var dividendId = _wallet.Dividends.Single().Id;

        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_bbas3, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync(
            $"v1/wallets/{_wallet.Id}/dividends/{dividendId}/reverse", null, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(x => x.Dividends)
            .ThenInclude(d => d.Asset)
            .FirstOrDefaultAsync(x => x.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.IsTrue(storedWallet.Dividends.Single().IsReversed);
        Assert.IsNotNull(storedWallet.Dividends.Single().ReversedAt);
        Assert.AreEqual(0m, storedWallet.GetTotalDividends().Value);
    }

    [TestMethod]
    [TestCategory("ReverseDividendEndpoint Tests")]
    public async Task Should_Return_NotFound_When_Dividend_Is_Not_In_Wallet()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_bbas3, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync(
            $"v1/wallets/{_wallet.Id}/dividends/{Guid.NewGuid()}/reverse", null, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}