using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroAffiliationSelector : ComponentBase, IDisposable
{
  [Inject]
  private IHeroClassClient HeroClassClient { get; set; } = default!;

  [Inject]
  private IMythologyTypeClient MythologyClient { get; set; } = default!;

  [Inject]
  private IOriginCultureClient CultureClient { get; set; } = default!;

  [Inject]
  private IOriginArchetypeClient ArchetypeClient { get; set; } = default!;

  [Inject]
  private ILogger<HeroAffiliationSelector> Logger { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public HeroCreateRequestDto Model { get; set; } = default!;

  private readonly CancellationTokenSource _lifetimeCts = new();
  private bool _disposed;

  private IReadOnlyCollection<HeroClassDto> HeroClasses = [];
  private IReadOnlyCollection<MythologyTypeDto> Mythologies = [];
  private IReadOnlyCollection<OriginCultureDto> Cultures = [];
  private IReadOnlyCollection<OriginArchetypeDto> Archetypes = [];

  private bool _loadingHeroClasses;
  private bool _loadingMythologies;
  private bool _loadingCultures;
  private bool _loadingArchetypes;

  private string? _heroClassesError;
  private string? _mythologiesError;
  private string? _culturesError;
  private string? _archetypesError;

  protected override Task OnInitializedAsync() => LoadAllAsync();

  private Task LoadAllAsync() => Task.WhenAll(
    LoadHeroClassesAsync(),
    LoadMythologiesAsync(),
    LoadCulturesAsync(),
    LoadArchetypesAsync());

  private async Task LoadHeroClassesAsync()
  {
    _loadingHeroClasses = true;
    _heroClassesError = null;
    try
    {
      HeroClasses = await HeroClassClient.GetAllAsync(_lifetimeCts.Token);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load hero classes for hero creation.");
      _heroClassesError = "Hero classes could not be loaded.";
    }
    finally
    {
      if (!_disposed)
        _loadingHeroClasses = false;
    }
  }

  private async Task LoadMythologiesAsync()
  {
    _loadingMythologies = true;
    _mythologiesError = null;
    try
    {
      Mythologies = await MythologyClient.GetAllAsync(_lifetimeCts.Token);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load mythologies for hero creation.");
      _mythologiesError = "Mythologies could not be loaded.";
    }
    finally
    {
      if (!_disposed)
        _loadingMythologies = false;
    }
  }

  private async Task LoadCulturesAsync()
  {
    _loadingCultures = true;
    _culturesError = null;
    try
    {
      Cultures = await CultureClient.GetAllAsync(_lifetimeCts.Token);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load origin cultures for hero creation.");
      _culturesError = "Origin cultures could not be loaded.";
    }
    finally
    {
      if (!_disposed)
        _loadingCultures = false;
    }
  }

  private async Task LoadArchetypesAsync()
  {
    _loadingArchetypes = true;
    _archetypesError = null;
    try
    {
      Archetypes = await ArchetypeClient.GetAllAsync(_lifetimeCts.Token);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load origin archetypes for hero creation.");
      _archetypesError = "Origin archetypes could not be loaded.";
    }
    finally
    {
      if (!_disposed)
        _loadingArchetypes = false;
    }
  }

  public void Dispose()
  {
    if (_disposed)
      return;

    _disposed = true;
    _lifetimeCts.Cancel();
    _lifetimeCts.Dispose();
  }
}
