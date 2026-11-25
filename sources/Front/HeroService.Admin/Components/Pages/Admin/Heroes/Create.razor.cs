using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class Create : ComponentBase, IDisposable
{
  [Inject]
  private IHeroClient HeroClient { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  [Inject]
  private NavigationManager NavigationManager { get; set; } = default!;

  [Inject]
  private ILogger<Create> Logger { get; set; } = default!;

  private readonly CancellationTokenSource _lifetimeCts = new();
  private HeroCreateRequestDto Model = new();
  private IReadOnlyList<string> _validationErrors = [];
  private string? _errorMessage;
  private bool _isSubmitting;
  private bool _disposed;

  private async Task CreateHero()
  {
    if (_isSubmitting || _disposed)
      return;

    _validationErrors = ValidateRequest(Model);
    _errorMessage = null;

    if (_validationErrors.Count > 0)
      return;

    Model.Name = Model.Name.Trim();
    _isSubmitting = true;

    try
    {
      var heroId = await HeroClient.CreateAsync(Model, _lifetimeCts.Token);

      if (heroId == Guid.Empty)
      {
        _errorMessage = "The API did not return a valid hero identifier. The hero may not have been created.";
        return;
      }

      if (_disposed)
        return;

      Snackbar.Add($"Hero '{Model.Name}' was created successfully.", Severity.Success);
      NavigationManager.NavigateTo("/admin/heroes");
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
      // Navigation or component disposal cancelled the in-flight request.
    }
    catch (OperationCanceledException exception)
    {
      Logger.LogWarning(exception, "Hero creation timed out.");
      _errorMessage = "The request timed out. Check the service connection before trying again.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API rejected or could not process hero creation.");
      _errorMessage = "The hero could not be created. Check the supplied information and service connection.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while creating hero {HeroName}.", Model.Name);
      _errorMessage = "An unexpected error occurred while creating the hero.";
    }
    finally
    {
      if (!_disposed)
        _isSubmitting = false;
    }
  }

  private static IReadOnlyList<string> ValidateRequest(HeroCreateRequestDto request)
  {
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(request.Name))
      errors.Add("A hero name is required.");

    if (request.HeroClassId == Guid.Empty)
      errors.Add("Select a hero class.");

    if (request.MythologyTypeId == Guid.Empty)
      errors.Add("Select a mythology.");

    if (request.OriginCultureId == Guid.Empty)
      errors.Add("Select an origin culture.");

    if (request.OriginArchetypeId == Guid.Empty)
      errors.Add("Select an origin archetype.");

    if (request.SkillKit.PassiveSkillId == Guid.Empty ||
        request.SkillKit.PrimarySkillId == Guid.Empty ||
        request.SkillKit.SecondarySkillId == Guid.Empty ||
        request.SkillKit.TertiarySkillId == Guid.Empty ||
        request.SkillKit.UltimateSkillId == Guid.Empty)
      errors.Add("Assign a valid skill to all five skill-kit slots.");

    return errors;
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
