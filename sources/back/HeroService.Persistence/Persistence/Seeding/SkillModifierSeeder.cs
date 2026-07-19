using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Version-scoped balance multipliers for all 90 skills, seeded against
/// GameVersion 1.0.0. All multipliers are neutral (1.0f = no change from
/// SkillBaseStats) -- the codex only supplies base stats, not a balance-patch
/// delta, so this is the "1.0.0 as shipped, unpatched" baseline. A future
/// rebalance seeder for a new GameVersion is where real per-skill
/// multipliers would live.
/// </summary>
public sealed class SkillModifierSeeder : ISeeder
{
  public int Order => 5;
  private readonly IEntityFactory<Guid, SkillModifier> _factory;
  private readonly IEntityRepository<SkillModifier, Guid> _repo;
  private readonly ISkillRepository _skills;
  private readonly IGameVersionRepository _versions;
  private readonly IUnitOfWork _uow;

  public SkillModifierSeeder(
      IEntityFactory<Guid, SkillModifier> factory,
      IEntityRepository<SkillModifier, Guid> repo,
      ISkillRepository skills,
      IGameVersionRepository versions,
      IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _skills = skills;
    _versions = versions;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    var version = await _versions.GetByVersionNumberAsync("1.0.0", ct)
        ?? throw new InvalidOperationException("Missing GameVersion 1.0.0");

    // =====================================================
    // THOR
    // =====================================================
    var lightningCharge = await GetSkill("Lightning Charge", ct);
    var thunderStrike = await GetSkill("Thunder Strike", ct);
    var hammersCall = await GetSkill("Hammer's Call", ct);
    var asgardsGuard = await GetSkill("Asgard's Guard", ct);
    var asgardsWrath = await GetSkill("Asgard's Wrath", ct);

    // =====================================================
    // ARES
    // =====================================================
    var bloodlust = await GetSkill("Bloodlust", ct);
    var bloodHarvest = await GetSkill("Blood Harvest", ct);
    var butchersCharge = await GetSkill("Butcher's Charge", ct);
    var warCry = await GetSkill("War Cry", ct);
    var warsFury = await GetSkill("War's Fury", ct);

    // =====================================================
    // SUSANOO
    // =====================================================
    var typhoonsBreath = await GetSkill("Typhoon's Breath", ct);
    var galeBlade = await GetSkill("Gale Blade", ct);
    var typhoonStep = await GetSkill("Typhoon Step", ct);
    var cuttingWind = await GetSkill("Cutting Wind", ct);
    var orochi = await GetSkill("Orochi", ct);

    // =====================================================
    // LOKI
    // =====================================================
    var deceit = await GetSkill("Deceit", ct);
    var twinBlades = await GetSkill("Twin Blades", ct);
    var shadowFlee = await GetSkill("Shadow Flee", ct);
    var poisonedDagger = await GetSkill("Poisoned Dagger", ct);
    var trickstersVerdict = await GetSkill("Trickster's Verdict", ct);

    // =====================================================
    // NYX
    // =====================================================
    var childofNight = await GetSkill("Child of Night", ct);
    var nightfallVeil = await GetSkill("Nightfall Veil", ct);
    var starstep = await GetSkill("Starstep", ct);
    var shadowClaws = await GetSkill("Shadow Claws", ct);
    var everlastingNight = await GetSkill("Everlasting Night", ct);

    // =====================================================
    // SET
    // =====================================================
    var desertVenom = await GetSkill("Desert Venom", ct);
    var desertsBreath = await GetSkill("Desert's Breath", ct);
    var jackalsHunt = await GetSkill("Jackal's Hunt", ct);
    var corrosiveSands = await GetSkill("Corrosive Sands", ct);
    var curseofChaos = await GetSkill("Curse of Chaos", ct);

    // =====================================================
    // ZEUS
    // =====================================================
    var ostatic = await GetSkill("Static", ct);
    var targetedBolt = await GetSkill("Targeted Bolt", ct);
    var olympusGale = await GetSkill("Olympus Gale", ct);
    var electricArc = await GetSkill("Electric Arc", ct);
    var skysFury = await GetSkill("Sky's Fury", ct);

    // =====================================================
    // AMUN-RA
    // =====================================================
    var radiance = await GetSkill("Radiance", ct);
    var solarDisc = await GetSkill("Solar Disc", ct);
    var celestialBarque = await GetSkill("Celestial Barque", ct);
    var sacredBurn = await GetSkill("Sacred Burn", ct);
    var eternalNoon = await GetSkill("Eternal Noon", ct);

    // =====================================================
    // CHRONOS
    // =====================================================
    var timeSlip = await GetSkill("Time Slip", ct);
    var hourFracture = await GetSkill("Hour Fracture", ct);
    var temporalLeap = await GetSkill("Temporal Leap", ct);
    var temporalShard = await GetSkill("Temporal Shard", ct);
    var hourglassReversal = await GetSkill("Hourglass Reversal", ct);

    // =====================================================
    // HERAKLES
    // =====================================================
    var titanicStrength = await GetSkill("Titanic Strength", ct);
    var nemeanGrip = await GetSkill("Nemean Grip", ct);
    var lionsCharge = await GetSkill("Lion's Charge", ct);
    var lionsSkin = await GetSkill("Lion's Skin", ct);
    var twelveLabors = await GetSkill("Twelve Labors", ct);

    // =====================================================
    // ANUBIS
    // =====================================================
    var guardianoftheThreshold = await GetSkill("Guardian of the Threshold", ct);
    var judgmentsVeil = await GetSkill("Judgment's Veil", ct);
    var passageofShadows = await GetSkill("Passage of Shadows", ct);
    var funeraryScepter = await GetSkill("Funerary Scepter", ct);
    var weighingofSouls = await GetSkill("Weighing of Souls", ct);

    // =====================================================
    // HEL
    // =====================================================
    var chilloftheDead = await GetSkill("Chill of the Dead", ct);
    var graspoftheFallen = await GetSkill("Grasp of the Fallen", ct);
    var walkoftheDead = await GetSkill("Walk of the Dead", ct);
    var icyBreath = await GetSkill("Icy Breath", ct);
    var domainofHelheim = await GetSkill("Domain of Helheim", ct);

    // =====================================================
    // ISIS
    // =====================================================
    var ancientMagic = await GetSkill("Ancient Magic", ct);
    var healingWing = await GetSkill("Healing Wing", ct);
    var isissFlight = await GetSkill("Isis's Flight", ct);
    var rayofLight = await GetSkill("Ray of Light", ct);
    var osirissReprieve = await GetSkill("Osiris's Reprieve", ct);

    // =====================================================
    // FREYJA
    // =====================================================
    var valkyriesFavor = await GetSkill("Valkyries' Favor", ct);
    var valkyriesBlessing = await GetSkill("Valkyries' Blessing", ct);
    var falconFlight = await GetSkill("Falcon Flight", ct);
    var goldenBlade = await GetSkill("Golden Blade", ct);
    var fieldofFolkvangr = await GetSkill("Field of Folkvangr", ct);

    // =====================================================
    // ENKI
    // =====================================================
    var watersWisdom = await GetSkill("Water's Wisdom", ct);
    var streamofWisdom = await GetSkill("Stream of Wisdom", ct);
    var undercurrent = await GetSkill("Undercurrent", ct);
    var invigoratingWave = await GetSkill("Invigorating Wave", ct);
    var abzu = await GetSkill("Abzu", ct);

    // =====================================================
    // ARTEMIS
    // =====================================================
    var huntresssEye = await GetSkill("Huntress's Eye", ct);
    var lunarArrow = await GetSkill("Lunar Arrow", ct);
    var doesLeap = await GetSkill("Doe's Leap", ct);
    var rainofArrows = await GetSkill("Rain of Arrows", ct);
    var sacredHunt = await GetSkill("Sacred Hunt", ct);

    // =====================================================
    // VIDAR
    // =====================================================
    var avengingSilence = await GetSkill("Avenging Silence", ct);
    var silencingShaft = await GetSkill("Silencing Shaft", ct);
    var ironStride = await GetSkill("Iron Stride", ct);
    var crushingBoot = await GetSkill("Crushing Boot", ct);
    var fenrirsVengeance = await GetSkill("Fenrir's Vengeance", ct);

    // =====================================================
    // RAMA
    // =====================================================
    var discipline = await GetSkill("Discipline", ct);
    var arrowofDharma = await GetSkill("Arrow of Dharma", ct);
    var princesStride = await GetSkill("Prince's Stride", ct);
    var chainedShot = await GetSkill("Chained Shot", ct);
    var brahmastra = await GetSkill("Brahmastra", ct);

    // Neutral (1.0f) multipliers for every skill -- see class remarks.
    await CreateAsync(version.Id, lightningCharge.Id, ct);
    await CreateAsync(version.Id, thunderStrike.Id, ct);
    await CreateAsync(version.Id, hammersCall.Id, ct);
    await CreateAsync(version.Id, asgardsGuard.Id, ct);
    await CreateAsync(version.Id, asgardsWrath.Id, ct);
    await CreateAsync(version.Id, bloodlust.Id, ct);
    await CreateAsync(version.Id, bloodHarvest.Id, ct);
    await CreateAsync(version.Id, butchersCharge.Id, ct);
    await CreateAsync(version.Id, warCry.Id, ct);
    await CreateAsync(version.Id, warsFury.Id, ct);
    await CreateAsync(version.Id, typhoonsBreath.Id, ct);
    await CreateAsync(version.Id, galeBlade.Id, ct);
    await CreateAsync(version.Id, typhoonStep.Id, ct);
    await CreateAsync(version.Id, cuttingWind.Id, ct);
    await CreateAsync(version.Id, orochi.Id, ct);
    await CreateAsync(version.Id, deceit.Id, ct);
    await CreateAsync(version.Id, twinBlades.Id, ct);
    await CreateAsync(version.Id, shadowFlee.Id, ct);
    await CreateAsync(version.Id, poisonedDagger.Id, ct);
    await CreateAsync(version.Id, trickstersVerdict.Id, ct);
    await CreateAsync(version.Id, childofNight.Id, ct);
    await CreateAsync(version.Id, nightfallVeil.Id, ct);
    await CreateAsync(version.Id, starstep.Id, ct);
    await CreateAsync(version.Id, shadowClaws.Id, ct);
    await CreateAsync(version.Id, everlastingNight.Id, ct);
    await CreateAsync(version.Id, desertVenom.Id, ct);
    await CreateAsync(version.Id, desertsBreath.Id, ct);
    await CreateAsync(version.Id, jackalsHunt.Id, ct);
    await CreateAsync(version.Id, corrosiveSands.Id, ct);
    await CreateAsync(version.Id, curseofChaos.Id, ct);
    await CreateAsync(version.Id, ostatic.Id, ct);
    await CreateAsync(version.Id, targetedBolt.Id, ct);
    await CreateAsync(version.Id, olympusGale.Id, ct);
    await CreateAsync(version.Id, electricArc.Id, ct);
    await CreateAsync(version.Id, skysFury.Id, ct);
    await CreateAsync(version.Id, radiance.Id, ct);
    await CreateAsync(version.Id, solarDisc.Id, ct);
    await CreateAsync(version.Id, celestialBarque.Id, ct);
    await CreateAsync(version.Id, sacredBurn.Id, ct);
    await CreateAsync(version.Id, eternalNoon.Id, ct);
    await CreateAsync(version.Id, timeSlip.Id, ct);
    await CreateAsync(version.Id, hourFracture.Id, ct);
    await CreateAsync(version.Id, temporalLeap.Id, ct);
    await CreateAsync(version.Id, temporalShard.Id, ct);
    await CreateAsync(version.Id, hourglassReversal.Id, ct);
    await CreateAsync(version.Id, titanicStrength.Id, ct);
    await CreateAsync(version.Id, nemeanGrip.Id, ct);
    await CreateAsync(version.Id, lionsCharge.Id, ct);
    await CreateAsync(version.Id, lionsSkin.Id, ct);
    await CreateAsync(version.Id, twelveLabors.Id, ct);
    await CreateAsync(version.Id, guardianoftheThreshold.Id, ct);
    await CreateAsync(version.Id, judgmentsVeil.Id, ct);
    await CreateAsync(version.Id, passageofShadows.Id, ct);
    await CreateAsync(version.Id, funeraryScepter.Id, ct);
    await CreateAsync(version.Id, weighingofSouls.Id, ct);
    await CreateAsync(version.Id, chilloftheDead.Id, ct);
    await CreateAsync(version.Id, graspoftheFallen.Id, ct);
    await CreateAsync(version.Id, walkoftheDead.Id, ct);
    await CreateAsync(version.Id, icyBreath.Id, ct);
    await CreateAsync(version.Id, domainofHelheim.Id, ct);
    await CreateAsync(version.Id, ancientMagic.Id, ct);
    await CreateAsync(version.Id, healingWing.Id, ct);
    await CreateAsync(version.Id, isissFlight.Id, ct);
    await CreateAsync(version.Id, rayofLight.Id, ct);
    await CreateAsync(version.Id, osirissReprieve.Id, ct);
    await CreateAsync(version.Id, valkyriesFavor.Id, ct);
    await CreateAsync(version.Id, valkyriesBlessing.Id, ct);
    await CreateAsync(version.Id, falconFlight.Id, ct);
    await CreateAsync(version.Id, goldenBlade.Id, ct);
    await CreateAsync(version.Id, fieldofFolkvangr.Id, ct);
    await CreateAsync(version.Id, watersWisdom.Id, ct);
    await CreateAsync(version.Id, streamofWisdom.Id, ct);
    await CreateAsync(version.Id, undercurrent.Id, ct);
    await CreateAsync(version.Id, invigoratingWave.Id, ct);
    await CreateAsync(version.Id, abzu.Id, ct);
    await CreateAsync(version.Id, huntresssEye.Id, ct);
    await CreateAsync(version.Id, lunarArrow.Id, ct);
    await CreateAsync(version.Id, doesLeap.Id, ct);
    await CreateAsync(version.Id, rainofArrows.Id, ct);
    await CreateAsync(version.Id, sacredHunt.Id, ct);
    await CreateAsync(version.Id, avengingSilence.Id, ct);
    await CreateAsync(version.Id, silencingShaft.Id, ct);
    await CreateAsync(version.Id, ironStride.Id, ct);
    await CreateAsync(version.Id, crushingBoot.Id, ct);
    await CreateAsync(version.Id, fenrirsVengeance.Id, ct);
    await CreateAsync(version.Id, discipline.Id, ct);
    await CreateAsync(version.Id, arrowofDharma.Id, ct);
    await CreateAsync(version.Id, princesStride.Id, ct);
    await CreateAsync(version.Id, chainedShot.Id, ct);
    await CreateAsync(version.Id, brahmastra.Id, ct);

    await _uow.CommitAsync(ct);
  }

  private async Task<SkillModifier> CreateAsync(
      Guid versionId,
      Guid skillId,
      CancellationToken ct,
      float cd = 1f,
      float mana = 1f,
      float dmg = 1f,
      float heal = 1f,
      float shield = 1f,
      float cast = 1f,
      float channel = 1f,
      float cc = 1f,
      float range = 1f)
  {
    var mod = _factory.Create();

    mod.Define(
      versionId,
      skillId,
      cd,
      mana,
      dmg,
      heal,
      shield,
      cast,
      channel,
      cc,
      range,
      "seed-system"
    );

    await _repo.AddAsync(mod, ct);

    return mod;
  }

  private async Task<Skill> GetSkill(string name, CancellationToken ct)
  {
    return await _skills.GetByNameAsync(name, ct)
        ?? throw new InvalidOperationException($"Missing skill: {name}");
  }
}