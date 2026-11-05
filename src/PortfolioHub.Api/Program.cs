using PortfolioHub.Api.Endpoints;
using PortfolioHub.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddDocumentation();
builder.AddServices();
builder.AddHttpJsonOptions();
builder.AddCrossOrigin();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(BuilderExtension.WebCorsPolicy);
app.MapEndpoints();
app.ConfigureDevEnvironment();

app.Run();

// Permite que os testes hospedem a API com WebApplicationFactory<Program>.
public partial class Program { }
