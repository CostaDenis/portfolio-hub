namespace PortfolioHub.Api.Endpoints.Abstractions;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}