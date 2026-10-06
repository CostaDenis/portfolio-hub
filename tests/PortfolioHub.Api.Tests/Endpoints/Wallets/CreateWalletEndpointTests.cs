using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioHub.Api.Contracts.Wallets;

namespace PortfolioHub.Api.Tests.Endpoints.Wallets;

[TestClass]
public class CreateWalletEndpointTests : ApiTestBase
{
    [TestMethod]
    [TestCategory("CreateWalletEndpoint tests")]
    public async Task Shoul_Return_Created_When_Request_Is_Valid()
    {
        var request = new CreateWalletRequest("Criptomoedas");
        var response = await Client
            .PostAsJsonAsync("v1/wallets", request, CancellationToken.None);

        await using var verificationContext = CreateDbContext();
        var storedWallet = await verificationContext.Wallets.FirstOrDefaultAsync(x => x.Name == "Criptomoedas");

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(storedWallet);
    }

    [TestMethod]
    [TestCategory("CreateWalletEndpoint tests")]
    [DataRow(null)]
    [DataRow("A")]
    [DataRow("AAAAAAAAAAAAAAAAAAAAAAAAA")]
    public async Task Shoul_Return_BadRequest_When_Request_Is_Invalid(string? name)
    {
        var request = new CreateWalletRequest(name ?? string.Empty);
        var response = await Client
            .PostAsJsonAsync("v1/wallets", request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
