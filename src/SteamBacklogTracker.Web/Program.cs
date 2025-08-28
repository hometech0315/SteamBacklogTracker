using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;
using SteamBacklogTracker.Web;
using SteamBacklogTracker.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add configuration from appsettings.json
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddScoped(sp => http);

using var response = await http.GetAsync("appsettings.json");
using var stream = await response.Content.ReadAsStreamAsync();
builder.Configuration.AddJsonStream(stream);

// Configure API settings
var apiConfig = new ApiConfiguration();
builder.Configuration.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
builder.Services.Configure<ApiConfiguration>(
    builder.Configuration.GetSection(ApiConfiguration.SectionName));

// Add memory cache
builder.Services.AddMemoryCache();

// Configure MudBlazor services with Steam gaming theme
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Register API services
builder.Services.AddScoped<IApiErrorHandler, ApiErrorHandler>();
builder.Services.AddScoped<ILoadingStateService, LoadingStateService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ApiLoggingHandler>();

// Configure HTTP client and Refit services
ApiClientService.ConfigureServices(builder.Services, apiConfig);

await builder.Build().RunAsync();
