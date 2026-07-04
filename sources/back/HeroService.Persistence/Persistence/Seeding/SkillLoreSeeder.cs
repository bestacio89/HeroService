using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Persistence.Seeding;

public sealed class SkillLoreSeeder : ISeeder
{
  // NOTE: your SkillSeeder shows Order => 4 in the file you pasted, but my clone
  // of the repo has SkillSeeder at 3 and HeroSeeder at 4 — there's drift between
  // what I have and your local state. Confirm SkillSeeder's actual current Order
  // and set this to run immediately after it (lore depends on skills existing).
  public int Order => 9;

  private readonly IEntityFactory<Guid, SkillLore> _loreFactory;
  private readonly IEntityRepository<SkillLore, Guid> _loreRepo;
  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IUnitOfWork _uow;

  public SkillLoreSeeder(
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityRepository<SkillLore, Guid> loreRepo,
      IEntityRepository<Skill, Guid> skills,
      IUnitOfWork uow)
  {
    _loreFactory = loreFactory;
    _loreRepo = loreRepo;
    _skills = skills;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _loreRepo.GetAllAsync(ct)).Any())
      return;

    var skillsByName = (await _skills.GetAllAsync(ct))
        .ToDictionary(s => s.Name, s => s.Id);

    var lore = new[]
    {
      CreateLore(skillsByName, "Mjolnir Strike",
        "Thor slams Mjolnir into the ground, channeling the raw force of the storm through the earth itself.",
        "A golden shockwave ripples outward from the impact point, arcing with blue-white lightning."),

      CreateLore(skillsByName, "Thunder Leap",
        "Thor hurls himself forward on a bolt of lightning, closing the distance before his enemies can react.",
        "A streak of electric-blue light trails behind Thor as he arcs through the air, thunder cracking on landing."),

      CreateLore(skillsByName, "Storm Aura",
        "A crackling field of static gathers around Thor, the pressure of an oncoming storm made manifest as protection.",
        "A translucent gold-blue barrier hums around Thor, small arcs of lightning skittering across its surface."),

      CreateLore(skillsByName, "Lightning Chain",
        "Thor releases a bolt that leaps from foe to foe, the storm seeking out every exposed target in reach.",
        "A jagged white-blue bolt forks and rebounds between enemies, each jump brighter than the last."),

      CreateLore(skillsByName, "God of Thunder",
        "Thor calls down the full wrath of the storm, unleashing a devastation that answers to no mortal scale.",
        "The sky darkens overhead as a colossal bolt descends, engulfing the battlefield in blinding white light."),

      CreateLore(skillsByName, "Lion's Might",
        "Herakles draws on the strength of the Nemean Lion, its invulnerable hide settling over his shoulders like armor.",
        "A tawny, spectral lion's mane flares briefly around Herakles' frame as his muscles surge with golden light."),

      CreateLore(skillsByName, "Hydra Strike",
        "Herakles strikes with the ferocity of the Lernaean Hydra, each blow threatening to multiply into more.",
        "Herakles' weapon trails afterimages on the swing, each ghostly copy landing a fraction of a second apart."),

      CreateLore(skillsByName, "Titan Grip",
        "Herakles plants his feet and braces with the strength that once held the heavens on Atlas's behalf.",
        "A stony, bronze-tinted barrier locks into place around Herakles, veined with faint cracks of light."),

      CreateLore(skillsByName, "Labors Rush",
        "Driven by the same relentless will that carried him through his Twelve Labors, Herakles surges forward.",
        "Herakles charges low and fast, kicking up a trail of dust and faint golden motes with every stride."),

      CreateLore(skillsByName, "Divine Endurance",
        "Herakles calls upon the endurance that ultimately earned him a place among the gods, refusing to fall.",
        "A warm golden light suffuses Herakles' entire body, wounds visibly slowing their toll as he stands firm."),
    };

    foreach (var entry in lore)
    {
      if (entry is not null)
        await _loreRepo.AddAsync(entry, ct);
    }

    await _uow.CommitAsync(ct);
  }

  // -------------------------
  // Aggregate construction only
  // -------------------------
  private SkillLore? CreateLore(
      Dictionary<string, Guid> skillsByName,
      string skillName,
      string description,
      string visualExplanation)
  {
    if (!skillsByName.TryGetValue(skillName, out var skillId))
      return null;

    var lore = _loreFactory.Create();
    lore.Define(skillId, description, visualExplanation, "skill-lore-seeder");
    return lore;
  }
}