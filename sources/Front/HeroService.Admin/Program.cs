using Franz.Common.Logging.Extensions;
using HeroService.Admin.Components;
using HeroService.Admin.Logging;
using HeroService.Admin.Operational;
using HeroService.Admin.UI;
using HeroService.Client.Http.DependencyInjection;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// RAZOR COMPONENTS
// =========================================
BlazorLoggingConfiguration.Configure(builder.Configuration);
builder.Host.UseLog();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// =========================================
// MUD BLAZOR
// =========================================
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = "mud-snackbar-location-bottom-right";
});
builder.Services.AddSingleton(GameTheme.Create());

// =========================================
// HERO SERVICE HTTP CLIENTS
// =========================================
var heroServiceBaseUrl = builder.Configuration["HeroService:BaseUrl"]
                         ?? "https://localhost:5001";

builder.Services.AddHeroServiceClients(
    baseUrl: heroServiceBaseUrl,
    configureHttpClient: client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("X-Client", "HeroService.Admin");
    });

// =========================================
// OPERATIONAL VISIBILITY HTTP CLIENTS
// =========================================
builder.Services.AddHttpClient(OperationalStatusClient.ApiClientName, client =>
{
    client.BaseAddress = new Uri(heroServiceBaseUrl, UriKind.Absolute);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpClient(OperationalStatusClient.ConsumerClientName, client =>
{
    var consumerBaseUrl = builder.Configuration["OperationalVisibility:ConsumerBaseUrl"];

    if (Uri.TryCreate(consumerBaseUrl, UriKind.Absolute, out var consumerUri))
    {
        client.BaseAddress = consumerUri;
    }

    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<IOperationalStatusClient, OperationalStatusClient>();

var app = builder.Build();

// =========================================
// PIPELINE
// =========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
