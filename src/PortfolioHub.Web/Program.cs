using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PortfolioHub.Web;
using PortfolioHub.Web.Api;
using PortfolioHub.Web.Api.Abstractions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Configure 'Api:BaseUrl' em wwwroot/appsettings.json.");

builder.Services.AddHttpClient<IWalletApiClient, WalletApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IAssetApiClient, AssetApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));

var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await builder.Build().RunAsync();
