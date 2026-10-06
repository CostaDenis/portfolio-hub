using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class BuyAssetEndpointTests : ApiTestBase
{

    private readonly Wallet _wallet;
    private readonly Asset _xpml11;

    public BuyAssetEndpointTests()
    {
        _wallet = new Wallet(new WalletName("FIIs"));

        _xpml11 = new Asset(new AssetName("XP Malls"), new Ticker("XPML11"),
            EAssetType.RealStateFund, new MarketPrice(new Money(105.12345678m)));
    }

    [TestMethod]
    [TestCategory("BuyAssetEndpoint tests")]
    public async Task Should_Return_NoContent_When_Request_Is_Valid()
    {
        await using (var context = CreateDbContext())
        {
            context.Wallets.Add(_wallet);
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new BuyAssetRequest(_xpml11.Id, 5m, 110m);
        var response = await Client
            .PostAsJsonAsync($"v1/wallets/{_wallet.Id}/assets/buy", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .Include(wallet => wallet.Transactions)
                .ThenInclude(transaction => transaction.Asset)
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        var assetQuantity = storedWallet!.GetCurrentQuantity(_xpml11);
        var assetTransactions = storedWallet!.Transactions.Where(x => x.Asset.Id == _xpml11.Id);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.AreEqual(new Quantity(5m), assetQuantity);
        Assert.IsNotNull(assetTransactions);
    }

    [TestMethod]
    [TestCategory("BuyAssetEndpoint tests")]
    public async Task Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        await using (var context = CreateDbContext())
        {
            context.Wallets.Add(_wallet);
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        //Preço inválido
        var request = new BuyAssetRequest(_xpml11.Id, 5m, -110m);
        var response = await Client
            .PostAsJsonAsync($"v1/wallets/{_wallet.Id}/assets/buy", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("BuyAssetEndpoint tests")]
    public async Task Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            context.Wallets.Add(_wallet);
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        //Asset não encontrado
        var request = new BuyAssetRequest(Guid.NewGuid(), 5m, -110m);
        var response = await Client
            .PostAsJsonAsync($"v1/wallets/{_wallet.Id}/assets/buy", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("BuyAssetEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        await using (var context = CreateDbContext())
        {
            context.Wallets.Add(_wallet);
            context.Assets.Add(_xpml11);
            await context.SaveChangesAsync(CancellationToken.None);
        }
        var request = new BuyAssetRequest(_xpml11.Id, 5m, -110m);
        //Wallet não encontrado
        var response = await Client
            .PostAsJsonAsync($"v1/wallets/{Guid.NewGuid()}/assets/buy", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
