using HeroService.Admin.Components;
using HeroService.Client.Http.DependencyInjection;
using MudBlazor.Services;
using HeroService.Admin.UI;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// RAZOR COMPONENTS
// =========================================
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
builder.Services.AddHeroServiceClients(
    baseUrl: builder.Configuration["HeroService:BaseUrl"]
              ?? "https://localhost:5001",
    configureHttpClient: client =>
    {
      client.Timeout = TimeSpan.FromSeconds(30);
      client.DefaultRequestHeaders.Add("X-Client", "HeroService.Admin");
    });

// =========================================
// UI SERVICES (future-ready)
// =========================================


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

// static assets (Blazor Web App pattern)
app.MapStaticAssets();

// Razor app entry
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();