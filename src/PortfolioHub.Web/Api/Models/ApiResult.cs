namespace PortfolioHub.Web.Api.Models;

public record class ApiResult(string? Error)
{
    public bool Succeeded => Error is null;
    public static ApiResult Ok() => new((string?)null);
    public static ApiResult Fail(string error) => new(error);
}

public record ApiResult<T>(T? Value, string? Error)
{
    public bool Succeeded => Error is null;
    public static ApiResult<T> Ok(T value) => new(value, null);
    public static ApiResult<T> Fail(string error) => new(default, error);
}
