using System.Net;
using System.Text;

namespace PortfolioHub.Infrastructure.Tests.Providers;

//simula a Internet
public class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    : HttpMessageHandler
{
    //todaa requisição recebe a mesma resposta
    public StubHttpMessageHandler(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        : this(_ => JsonResponse(json, statusCode))
    {
    }

    public List<HttpRequestMessage> Requests { get; } = [];

    public HttpRequestMessage? LastRequest => Requests.LastOrDefault();

    public static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        return Task.FromResult(respond(request));
    }
}
