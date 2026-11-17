using System.Net;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class ReverseTransactionEndpointTests : ApiTestBase
{

    private readonly Wallet _wallet;
    private readonly Asset _bbas3;

    public ReverseTransactionEndpointTests()
    {
        _wallet = new(new WalletName("Ações"));
        _bbas3 = new(new AssetName("Banco do Brasil"), new Ticker("BBAS3"),
            EAssetType.Stock, new MarketPrice(17m));

        _wallet.BuyAsset(_bbas3, 40, 16m);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionEndpoint Tests")]
    public async Task Should_Return_NoContent_And_Persist_Reversal_When_Transaction_Exists()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_bbas3, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var transactionId = _wallet.Transactions.Single().Id;

        using var response = await Client.PostAsync
            ($"v1/wallets/{_wallet.Id}/transactions/{transactionId}/reverse", null, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
               .Include(x => x.Transactions)
               .ThenInclude(t => t.Asset)
               .FirstOrDefaultAsync(x => x.Id == _wallet.Id, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.IsTrue(storedWallet.Transactions.Single().IsReversed);
        Assert.AreEqual(0, storedWallet.GetCurrentQuantity(_bbas3).Value);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionEndpoint Tests")]
    public async Task Should_Return_Conflict_When_Transaction_Is_Already_Reversed()
    {
        var transactionId = _wallet.Transactions.Single().Id;
        _wallet.ReverseTransaction(transactionId); //transação estornada

        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_bbas3, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync(
            $"v1/wallets/{_wallet.Id}/transactions/{transactionId}/reverse", null, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == _wallet.Id, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.IsTrue(storedWallet.Transactions.Single().IsReversed);
        Assert.AreEqual(_wallet.Transactions.Single().ReversedAt,
            storedWallet.Transactions.Single().ReversedAt);
    }

    [TestMethod]
    [TestCategory("ReverseTransactionEndpoint Tests")]
    public async Task Should_Return_NotFound_When_Transaction_Is_Not_In_Wallet()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.Assets.AddAsync(_bbas3, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client.PostAsync(
            $"v1/wallets/{_wallet.Id}/transactions/{Guid.NewGuid()}/reverse", null, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}