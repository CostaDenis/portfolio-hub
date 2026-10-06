using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class SellAssetEndpointTests : ApiTestBase
{
    private readonly Wallet _wallet;
    private readonly Asset _xpml11;

    public SellAssetEndpointTests()
    {
        _wallet = new Wallet(new WalletName("FIIs"));
        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(105m));
    }

    [TestMethod]
    [TestCategory("SellAssetEndpoint tests")]
    [DataRow(3)]
    [DataRow(10)]
    public async Task Should_Return_NoContent_And_Persist_Sale_When_Request_Is_Valid(int quantity)
    {
        _wallet.BuyAsset(_xpml11, 10m, 105m);
        var originalTransactionId = _wallet.Transactions.Single().Id;
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new SellAssetRequest(_xpml11.Id, quantity, 110m);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/sell", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Transactions).ThenInclude(transaction => transaction.Asset)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.AreEqual(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.IsNotNull(storedWallet);

        var buy = storedWallet.Transactions
            .Single(transaction => transaction.Id == originalTransactionId);

        Assert.HasCount(2, storedWallet.Transactions);
        Assert.AreEqual(ETransactionType.Buy, buy.Type);
        Assert.AreEqual(10m, buy.Quantity.Value);
        Assert.AreEqual(105m, buy.UnitPrice.Value);

        var sale = storedWallet.Transactions
            .Single(transaction => transaction.Type == ETransactionType.Sell);

        Assert.AreEqual(_xpml11.Id, sale.Asset.Id);
        Assert.AreEqual(quantity, sale.Quantity.Value);
        Assert.AreEqual(110m, sale.UnitPrice.Value);
        Assert.AreEqual(quantity * 110m, sale.Total.Value);
        Assert.AreEqual(10m - quantity, storedWallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("SellAssetEndpoint tests")]
    [DataRow(-1, 110)]
    [DataRow(3, -1)]
    public async Task Should_Return_BadRequest_Without_Persisting_Sale_When_Request_Is_Invalid(
        int quantity, int unitPrice)
    {
        _wallet.BuyAsset(_xpml11, 10m, 105m);
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new SellAssetRequest(_xpml11.Id, quantity, unitPrice);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/sell", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Transactions).ThenInclude(transaction => transaction.Asset)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.HasCount(1, storedWallet.Transactions);
        Assert.AreEqual(ETransactionType.Buy, storedWallet.Transactions.Single().Type);
        Assert.AreEqual(10m, storedWallet.GetCurrentQuantity(_xpml11).Value);
    }

    [TestMethod]
    [TestCategory("SellAssetEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new SellAssetRequest(_xpml11.Id, 3m, 110m);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{Guid.NewGuid()}/assets/sell", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("SellAssetEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new SellAssetRequest(Guid.NewGuid(), 3m, 110m);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/sell", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("SellAssetEndpoint tests")]
    public async Task Should_Return_Conflict_Without_Persisting_Sale_When_Quantity_Is_Insufficient()
    {
        _wallet.BuyAsset(_xpml11, 10m, 105m);
        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(_xpml11, CancellationToken.None);
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new SellAssetRequest(_xpml11.Id, 11m, 110m);
        using var response = await Client.PostAsJsonAsync(
            $"v1/wallets/{_wallet.Id}/assets/sell", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Transactions).ThenInclude(transaction => transaction.Asset)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.HasCount(1, storedWallet.Transactions);
        Assert.AreEqual(ETransactionType.Buy, storedWallet.Transactions.Single().Type);
        Assert.AreEqual(10m, storedWallet.GetCurrentQuantity(_xpml11).Value);
    }
}
