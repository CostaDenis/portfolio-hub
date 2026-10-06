using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Wallets;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class UpdateWalletNameEndpointTests : ApiTestBase
{
    private readonly Wallet _wallet = new(new WalletName("FIIs"));

    [TestMethod]
    [TestCategory("UpdateWalletNameEndpoint tests")]
    public async Task Should_Return_Ok_With_Wallet_Data_And_Persist_Updated_Name()
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new UpdateWalletNameRequest("Ações");
        using var response = await Client.PutAsJsonAsync(
            $"v1/wallets/{_wallet.Id}", request, CancellationToken.None);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var body = json.RootElement;
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.AreEqual(_wallet.Id, body.GetProperty("walletId").GetGuid());
        Assert.AreEqual("Ações", body.GetProperty("name").GetString());
        Assert.IsNotNull(storedWallet);
        Assert.AreEqual("Ações", storedWallet.Name.Value);
    }

    [TestMethod]
    [TestCategory("UpdateWalletNameEndpoint tests")]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("A")]
    [DataRow("123456789012345678901")]
    public async Task Should_Return_BadRequest_Without_Changing_Wallet_When_Name_Is_Invalid(string? name)
    {
        await using (var context = CreateDbContext())
        {
            await context.Wallets.AddAsync(_wallet, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        var request = new UpdateWalletNameRequest(name ?? string.Empty);
        using var response = await Client.PutAsJsonAsync(
            $"v1/wallets/{_wallet.Id}", request, CancellationToken.None);
        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets
            .FirstOrDefaultAsync(wallet => wallet.Id == _wallet.Id);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(storedWallet);
        Assert.AreEqual("FIIs", storedWallet.Name.Value);
    }

    [TestMethod]
    [TestCategory("UpdateWalletNameEndpoint tests")]
    public async Task Should_Return_NotFound_When_Wallet_Does_Not_Exist()
    {
        var request = new UpdateWalletNameRequest("Ações");
        using var response = await Client.PutAsJsonAsync(
            $"v1/wallets/{Guid.NewGuid()}", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
