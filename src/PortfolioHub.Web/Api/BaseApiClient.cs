using System.Net;
using System.Net.Http.Json;
using PortfolioHub.Web.Api.Models;

namespace PortfolioHub.Web.Api;

public abstract class BaseApiClient
{

    protected readonly HttpClient _client;

    protected BaseApiClient(IHttpClientFactory httpClientFactory)
        => _client = httpClientFactory.CreateClient();

    protected async Task<T?> GetOrDefaultAsync<T>(string uri, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    protected static async Task<ApiResult> ReadResultAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        var error = await ReadErrorAsync(responseMessage, cancellationToken);

        return error is null
            ? ApiResult.Ok()
            : ApiResult.Fail(error);
    }

    protected static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        var error = await ReadErrorAsync(responseMessage, cancellationToken);

        if (error is not null)
            return ApiResult<T>.Fail(error);

        var value = await responseMessage.Content.ReadFromJsonAsync<T>(cancellationToken);

        return ApiResult<T>.Ok(value!);
    }

    protected static async Task<string?> ReadErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {

        if (responseMessage.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Conflict))
        {
            //caso de sucesso
            responseMessage.EnsureSuccessStatusCode();
            return null;
        }

        var problem = await responseMessage.Content.ReadFromJsonAsync<ProblemResponse>(cancellationToken);

        return problem?.Detail ?? problem?.Title ?? "Não foi possível concluir a operação";
    }

    //TRequest -> convenção de nome para receber
    //tipo genérico de request
    protected async Task<ApiResult> PostAsync<TRequest>(string uri, TRequest request, CancellationToken cancellationToken)
    {
        using var response = await _client.PostAsJsonAsync(uri, request, cancellationToken);

        return await ReadResultAsync(response, cancellationToken);
    }
}
