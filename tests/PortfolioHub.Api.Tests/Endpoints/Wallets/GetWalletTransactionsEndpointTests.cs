using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletTransactionsEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("GetWalletTransactionsEndpoint tests")]
    public async Task Should_Return_Ok_With_WalletTransactions_Data()
    {
        var wallet = new Wallet(new WalletName("Ações"));
        var asset = new Asset(new AssetName("Banco do Brasil SA"), new Ticker("BBAS3"),
            EAssetType.Stock, new MarketPrice(23m));

        wallet.BuyAsset(asset, 10m, 23m);
        wallet.SellAsset(asset, 2m, 25m);
        var buy = wallet.Transactions.Single(transaction => transaction.Type == ETransactionType.Buy);
        var sell = wallet.Transactions.Single(transaction => transaction.Type == ETransactionType.Sell);

        await using (var context = CreateDbContext())
        {
            await context.Assets.AddAsync(asset, CancellationToken.None);
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}/transactions", CancellationToken.None);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var body = json.RootElement;
        var buyJson = body.EnumerateArray()
            .Single(item => item.GetProperty("transactionId").GetGuid() == buy.Id);
        var sellJson = body.EnumerateArray()
            .Single(item => item.GetProperty("transactionId").GetGuid() == sell.Id);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(JsonValueKind.Array, body.ValueKind);
        Assert.AreEqual(2, body.GetArrayLength());

        Assert.AreEqual(asset.Id, buyJson.GetProperty("assetId").GetGuid());
        Assert.AreEqual("BBAS3", buyJson.GetProperty("ticker").GetString());
        Assert.AreEqual("Buy", buyJson.GetProperty("type").GetString());
        Assert.AreEqual(buy.Date, buyJson.GetProperty("date").GetDateTime());
        Assert.AreEqual(10m, buyJson.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(23m, buyJson.GetProperty("unitPrice").GetDecimal());
        Assert.AreEqual(230m, buyJson.GetProperty("total").GetDecimal());

        Assert.AreEqual(asset.Id, sellJson.GetProperty("assetId").GetGuid());
        Assert.AreEqual("BBAS3", sellJson.GetProperty("ticker").GetString());
        Assert.AreEqual("Sell", sellJson.GetProperty("type").GetString());
        Assert.AreEqual(sell.Date, sellJson.GetProperty("date").GetDateTime());
        Assert.AreEqual(2m, sellJson.GetProperty("quantity").GetDecimal());
        Assert.AreEqual(25m, sellJson.GetProperty("unitPrice").GetDecimal());
        Assert.AreEqual(50m, sellJson.GetProperty("total").GetDecimal());
    }

    [TestMethod]
    [TestCategory("GetWalletTransactionsEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        using var response = await Client
            .GetAsync($"v1/wallets/{Guid.NewGuid()}/transactions", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("GetWalletTransactionsEndpoint tests")]
    public async Task Should_Return_Ok_With_Empty_Array_When_Wallet_Has_No_Transactions()
    {
        var wallet = new Wallet(new WalletName("Ações"));
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}/transactions", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.AreEqual(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.AreEqual(0, json.RootElement.GetArrayLength());
    }

    [TestMethod]
    [TestCategory("GetWalletTransactionsEndpoint tests")]
    [DataRow(true, false)]
    [DataRow(false, true)]
    [DataRow(true, true)]
    public async Task Should_Return_Only_Transactions_Matching_Filters(bool filterByAsset, bool filterByType)
    {
        var wallet = new Wallet(new WalletName("Ações"));
        var bbas = new Asset(new AssetName("Banco do Brasil SA"), new Ticker("BBAS3"),
            EAssetType.Stock, new MarketPrice(23m));
        var petr = new Asset(new AssetName("Petrobras"), new Ticker("PETR4"),
            EAssetType.Stock, new MarketPrice(35m));

        wallet.BuyAsset(bbas, 10m, 23m);
        wallet.SellAsset(bbas, 2m, 25m);
        wallet.BuyAsset(petr, 5m, 35m);
        wallet.SellAsset(petr, 1m, 36m);

        var expectedIds = wallet.Transactions
            .Where(transaction => !filterByAsset || transaction.Asset.Id == bbas.Id)
            .Where(transaction => !filterByType || transaction.Type == ETransactionType.Sell)
            .Select(transaction => transaction.Id).ToArray();

        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var filters = new List<string>();
        if (filterByAsset)
            filters.Add($"assetId={bbas.Id}");
        if (filterByType)
            filters.Add("type=Sell");

        using var response = await Client.GetAsync(
            $"v1/wallets/{wallet.Id}/transactions?{string.Join("&", filters)}", CancellationToken.None);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var returnedIds = json.RootElement.EnumerateArray()
            .Select(item => item.GetProperty("transactionId").GetGuid()).ToArray();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(JsonValueKind.Array, json.RootElement.ValueKind);
        CollectionAssert.AreEquivalent(expectedIds, returnedIds);
    }

    [TestMethod]
    [TestCategory("GetWalletTransactionsEndpoint tests")]
    [DataRow("startDate=2026-10-02&endDate=2026-10-01")]
    [DataRow("type=999")]
    public async Task Should_Return_BadRequest_When_Filter_Is_Invalid(string filter)
    {
        var wallet = new Wallet(new WalletName("Ações"));
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        using var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}/transactions?{filter}", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
