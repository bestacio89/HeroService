using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.MythologyTypes;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private IMythologyTypeClient MythologyTypeClient { get; set; } = default!;

  [Inject]
  private NavigationManager Nav { get; set; } = default!;

  private List<MythologyTypeDto> _items = [];

  protected override async Task OnInitializedAsync()
  {
    var result = await MythologyTypeClient.GetAllAsync();

    _items = result?.ToList() ?? [];
  }
}