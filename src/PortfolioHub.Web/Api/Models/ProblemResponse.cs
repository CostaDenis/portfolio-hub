namespace PortfolioHub.Web.Api.Models;

//Recebe o GlobalException da API
public record class ProblemResponse(string? Title, string? Detail);