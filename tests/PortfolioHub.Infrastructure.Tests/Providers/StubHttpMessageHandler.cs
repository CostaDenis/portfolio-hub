using System.Net;
using System.Text;

namespace PortfolioHub.Infrastructure.Tests.Providers;

//simula a Internet
public class StubHttpMessageHandler(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
