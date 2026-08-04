using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroClasses;

public sealed partial class HeroClassDialog : ComponentBase
{
  [Inject]
  private IHeroClassClient Client { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;

  [Parameter]
  public EventCallback OnSaved { get; set; }

  private string Name = string.Empty;

  private bool IsLoading;

  private bool IsSaveDisabled =>
      IsLoading || string.IsNullOrWhiteSpace(Name);

  private async Task Save()
  {
    if (IsLoading)
      return;

    IsLoading = true;

    try
    {
      await Client.CreateAsync(new CreateHeroClassCommand(Name));

      Snackbar.Add("Hero Class created", Severity.Success);

      if (OnSaved.HasDelegate)
      {
        await OnSaved.InvokeAsync();
      }

      MudDialog.Close();
    }
    catch (Exception ex)
    {
      Snackbar.Add($"Error: {ex.Message}", Severity.Error);
    }
    finally
    {
      IsLoading = false;
    }
  }

  private void Cancel()
  {
    MudDialog.Cancel();
  }
}