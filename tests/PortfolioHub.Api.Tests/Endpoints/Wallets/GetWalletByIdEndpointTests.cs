using System.Net;
using System.Text.Json;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class GetWalletByIdEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("GetWalletByIdEndpoint tests")]
    public async Task Should_Return_Ok_When_Wallet_Exists()
    {
        var wallet = new Wallet(new WalletName("Ações"));
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var response = await Client
            .GetAsync($"v1/wallets/{wallet.Id}", CancellationToken.None);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(CancellationToken.None));
        var body = json.RootElement;

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(wallet.Id, body.GetProperty("walletId").GetGuid());
        Assert.AreEqual("Ações", body.GetProperty("name").GetString());
    }

    [TestMethod]
    [TestCategory("GetWalletByIdEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        var response = await Client
            .GetAsync($"v1/wallets/{Guid.NewGuid()}", CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

}
