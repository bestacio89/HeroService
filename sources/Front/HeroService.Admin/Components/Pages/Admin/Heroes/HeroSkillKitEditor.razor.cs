using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroSkillKitEditor : ComponentBase, IDisposable
{
  [Inject]
  private ISkillClient SkillClient { get; set; } = default!;

  [Inject]
  private ILogger<HeroSkillKitEditor> Logger { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public HeroSkillKitCreateRequestDto Model { get; set; } = default!;

  private readonly CancellationTokenSource _lifetimeCts = new();
  private bool _disposed;
  private bool _isLoading;
  private string? _loadError;
  private IReadOnlyCollection<SkillDto> Skills = [];

  protected override Task OnInitializedAsync() => LoadSkillsAsync();

  private async Task LoadSkillsAsync()
  {
    if (_disposed)
      return;

    _isLoading = true;
    _loadError = null;

    try
    {
      Skills = await SkillClient.GetAllAsync(_lifetimeCts.Token);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load skills for hero creation.");
      _loadError = "The skill catalogue could not be loaded.";
    }
    finally
    {
      if (!_disposed)
        _isLoading = false;
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
