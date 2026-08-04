using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class Create : ComponentBase
{
  [Inject]
  private IHeroClient HeroClient { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  private HeroCreateRequestDto Model = new();

  private async Task CreateHero()
  {
    var id = await HeroClient.CreateAsync(Model);

    // Optional navigation later.
  }
}