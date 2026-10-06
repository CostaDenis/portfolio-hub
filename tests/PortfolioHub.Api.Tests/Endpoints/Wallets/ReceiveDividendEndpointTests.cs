using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class ReceiveDividendEndpointTests : ApiTestBase
{
    private readonly Wallet _wallet;
    private readonly Asset _xpml11;
    private readonly DateTime _date = DateTime.UtcNow;

    public ReceiveDividendEndpointTests()
    {
        _wallet = new Wallet(new WalletName("FIIs"));
        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(105m));
    }

    [TestMethod]
    [TestCategory("ReceiveDividendEndpoint tests")]
    public async Task Should_Return_NoContent_And_Persist_Dividend_When_Request_Is_Valid()
    {
        _wallet.BuyAsset(_xpml11, 10m, 105m);
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new ReceiveDividendRequest(_xpml11.Id, 0.92m, _date);

        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/receive-dividend", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.AreEqual(string.Empty, await response.Content.ReadAsStringAsync());

        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Dividends).ThenInclude(dividend => dividend.Asset)
            .Include(wallet => wallet.Transactions).ThenInclude(transaction => transaction.Asset)
            .AsSplitQuery()
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.IsNotNull(storedWallet);

        var dividend = storedWallet.Dividends.Single();

        Assert.HasCount(1, storedWallet.Dividends);
        Assert.AreEqual(_xpml11.Id, dividend.Asset.Id);
        Assert.AreEqual(10m, dividend.Quantity.Value);
        Assert.AreEqual(0.92m, dividend.ValuePerShare.Value);
        Assert.AreEqual(_date, dividend.Date);
        Assert.AreEqual(9.2m, dividend.Total.Value);
        Assert.AreEqual(9.2m, storedWallet.GetTotalDividends().Value);
        Assert.HasCount(1, storedWallet.Transactions);
        Assert.AreEqual(10m, storedWallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendEndpoint tests")]
    [DataRow(0)]
    [DataRow(-1)]
    public async Task Should_Return_BadRequest_Without_Persisting_Dividend_When_Value_Is_Invalid(int valuePerShare)
    {
        _wallet.BuyAsset(_xpml11, 10m, 105m);
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new ReceiveDividendRequest(_xpml11.Id, valuePerShare, _date);

        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/receive-dividend", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Dividends)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.HasCount(0, storedWallet.Dividends);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new ReceiveDividendRequest(_xpml11.Id, 0.92m, _date);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{Guid.NewGuid()}/assets/receive-dividend", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new ReceiveDividendRequest(Guid.NewGuid(), 0.92m, _date);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/receive-dividend", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("ReceiveDividendEndpoint tests")]
    public async Task Should_Return_Conflict_Without_Persisting_Dividend_When_Wallet_Has_No_Asset_Quantity()
    {
        await using (var context = CreateDbContext())
        {
            context.Wallets.Add(_wallet);
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new ReceiveDividendRequest(_xpml11.Id, 0.92m, _date);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/receive-dividend", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Dividends)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.HasCount(0, storedWallet.Dividends);
    }
}
