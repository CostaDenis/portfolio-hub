using PortfolioHub.Api.Endpoints;
using PortfolioHub.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddDocumentation();
builder.AddServices();
builder.AddHttpJsonOptions();

var app = builder.Build();

app.UseExceptionHandler();
app.MapEndpoints();
app.ConfigureDevEnvironment();

app.Run();
