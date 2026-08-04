using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroAffiliationSelector : ComponentBase
{
  [Inject]
  private IHeroClassClient HeroClassClient { get; set; } = default!;

  [Inject]
  private IMythologyTypeClient MythologyClient { get; set; } = default!;

  [Inject]
  private IOriginCultureClient CultureClient { get; set; } = default!;

  [Inject]
  private IOriginArchetypeClient ArchetypeClient { get; set; } = default!;

  [Parameter]
  public HeroCreateRequestDto Model { get; set; } = default!;

  private IReadOnlyCollection<HeroClassDto> HeroClasses = [];

  private IReadOnlyCollection<MythologyTypeDto> Mythologies = [];

  private IReadOnlyCollection<OriginCultureDto> Cultures = [];

  private IReadOnlyCollection<OriginArchetypeDto> Archetypes = [];

  protected override async Task OnInitializedAsync()
  {
    var heroClassesTask = HeroClassClient.GetAllAsync();
    var mythologiesTask = MythologyClient.GetAllAsync();
    var culturesTask = CultureClient.GetAllAsync();
    var archetypesTask = ArchetypeClient.GetAllAsync();

    await Task.WhenAll(
        heroClassesTask,
        mythologiesTask,
        culturesTask,
        archetypesTask);

    HeroClasses = await heroClassesTask;
    Mythologies = await mythologiesTask;
    Cultures = await culturesTask;
    Archetypes = await archetypesTask;
  }
}