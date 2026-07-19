using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Structural AD/AP/MaxHealth scaling profile for all 90 skills. Not
/// version-scoped -- this is a skill's identity (how it scales), not a
/// balance-patch axis. Each ratio is the SUM of that stat's ratio across
/// every SkillEffect on the skill (see SkillSeeder), i.e. the skill's total
/// scaling exposure to that stat.
/// </summary>
public sealed class SkillScalingSeeder : ISeeder
{
  public int Order => 7;

  private readonly IEntityFactory<Guid, SkillScalingModifier> _factory;
  private readonly IEntityRepository<SkillScalingModifier, Guid> _repo;
  private readonly ISkillRepository _skills;
  private readonly IUnitOfWork _uow;

  public SkillScalingSeeder(
      IEntityFactory<Guid, SkillScalingModifier> factory,
      IEntityRepository<SkillScalingModifier, Guid> repo,
      ISkillRepository skills,
      IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _skills = skills;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    // =====================================================
    // THOR
    // =====================================================
    var lightningCharge = await GetSkillOrFailAsync("Lightning Charge", ct);
    await CreateAsync(lightningCharge.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var thunderStrike = await GetSkillOrFailAsync("Thunder Strike", ct);
    await CreateAsync(thunderStrike.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var hammersCall = await GetSkillOrFailAsync("Hammer's Call", ct);
    await CreateAsync(hammersCall.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var asgardsGuard = await GetSkillOrFailAsync("Asgard's Guard", ct);
    await CreateAsync(asgardsGuard.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.15f, system, ct);
    var asgardsWrath = await GetSkillOrFailAsync("Asgard's Wrath", ct);
    await CreateAsync(asgardsWrath.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ARES
    // =====================================================
    var bloodlust = await GetSkillOrFailAsync("Bloodlust", ct);
    await CreateAsync(bloodlust.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var bloodHarvest = await GetSkillOrFailAsync("Blood Harvest", ct);
    await CreateAsync(bloodHarvest.Id, attackDamageRatio: 1.1f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var butchersCharge = await GetSkillOrFailAsync("Butcher's Charge", ct);
    await CreateAsync(butchersCharge.Id, attackDamageRatio: 0.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var warCry = await GetSkillOrFailAsync("War Cry", ct);
    await CreateAsync(warCry.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var warsFury = await GetSkillOrFailAsync("War's Fury", ct);
    await CreateAsync(warsFury.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.1f, system, ct);

    // =====================================================
    // SUSANOO
    // =====================================================
    var typhoonsBreath = await GetSkillOrFailAsync("Typhoon's Breath", ct);
    await CreateAsync(typhoonsBreath.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var galeBlade = await GetSkillOrFailAsync("Gale Blade", ct);
    await CreateAsync(galeBlade.Id, attackDamageRatio: 0.9f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var typhoonStep = await GetSkillOrFailAsync("Typhoon Step", ct);
    await CreateAsync(typhoonStep.Id, attackDamageRatio: 0.85f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var cuttingWind = await GetSkillOrFailAsync("Cutting Wind", ct);
    await CreateAsync(cuttingWind.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var orochi = await GetSkillOrFailAsync("Orochi", ct);
    await CreateAsync(orochi.Id, attackDamageRatio: 1.4f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // LOKI
    // =====================================================
    var deceit = await GetSkillOrFailAsync("Deceit", ct);
    await CreateAsync(deceit.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var twinBlades = await GetSkillOrFailAsync("Twin Blades", ct);
    await CreateAsync(twinBlades.Id, attackDamageRatio: 1.1f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shadowFlee = await GetSkillOrFailAsync("Shadow Flee", ct);
    await CreateAsync(shadowFlee.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var poisonedDagger = await GetSkillOrFailAsync("Poisoned Dagger", ct);
    await CreateAsync(poisonedDagger.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var trickstersVerdict = await GetSkillOrFailAsync("Trickster's Verdict", ct);
    await CreateAsync(trickstersVerdict.Id, attackDamageRatio: 1.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // NYX
    // =====================================================
    var childofNight = await GetSkillOrFailAsync("Child of Night", ct);
    await CreateAsync(childofNight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var nightfallVeil = await GetSkillOrFailAsync("Nightfall Veil", ct);
    await CreateAsync(nightfallVeil.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var starstep = await GetSkillOrFailAsync("Starstep", ct);
    await CreateAsync(starstep.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shadowClaws = await GetSkillOrFailAsync("Shadow Claws", ct);
    await CreateAsync(shadowClaws.Id, attackDamageRatio: 0.9f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var everlastingNight = await GetSkillOrFailAsync("Everlasting Night", ct);
    await CreateAsync(everlastingNight.Id, attackDamageRatio: 1.3f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // SET
    // =====================================================
    var desertVenom = await GetSkillOrFailAsync("Desert Venom", ct);
    await CreateAsync(desertVenom.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var desertsBreath = await GetSkillOrFailAsync("Desert's Breath", ct);
    await CreateAsync(desertsBreath.Id, attackDamageRatio: 0.7f, abilityPowerRatio: 0.2f, maxHealthRatio: 0f, system, ct);
    var jackalsHunt = await GetSkillOrFailAsync("Jackal's Hunt", ct);
    await CreateAsync(jackalsHunt.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var corrosiveSands = await GetSkillOrFailAsync("Corrosive Sands", ct);
    await CreateAsync(corrosiveSands.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0.3f, maxHealthRatio: 0f, system, ct);
    var curseofChaos = await GetSkillOrFailAsync("Curse of Chaos", ct);
    await CreateAsync(curseofChaos.Id, attackDamageRatio: 1.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ZEUS
    // =====================================================
    var ostatic = await GetSkillOrFailAsync("Static", ct);
    await CreateAsync(ostatic.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var targetedBolt = await GetSkillOrFailAsync("Targeted Bolt", ct);
    await CreateAsync(targetedBolt.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.1f, maxHealthRatio: 0f, system, ct);
    var olympusGale = await GetSkillOrFailAsync("Olympus Gale", ct);
    await CreateAsync(olympusGale.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var electricArc = await GetSkillOrFailAsync("Electric Arc", ct);
    await CreateAsync(electricArc.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var skysFury = await GetSkillOrFailAsync("Sky's Fury", ct);
    await CreateAsync(skysFury.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.9f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // AMUN-RA
    // =====================================================
    var radiance = await GetSkillOrFailAsync("Radiance", ct);
    await CreateAsync(radiance.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var solarDisc = await GetSkillOrFailAsync("Solar Disc", ct);
    await CreateAsync(solarDisc.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.0f, maxHealthRatio: 0f, system, ct);
    var celestialBarque = await GetSkillOrFailAsync("Celestial Barque", ct);
    await CreateAsync(celestialBarque.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.4f, maxHealthRatio: 0f, system, ct);
    var sacredBurn = await GetSkillOrFailAsync("Sacred Burn", ct);
    await CreateAsync(sacredBurn.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var eternalNoon = await GetSkillOrFailAsync("Eternal Noon", ct);
    await CreateAsync(eternalNoon.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.7f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // CHRONOS
    // =====================================================
    var timeSlip = await GetSkillOrFailAsync("Time Slip", ct);
    await CreateAsync(timeSlip.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var hourFracture = await GetSkillOrFailAsync("Hour Fracture", ct);
    await CreateAsync(hourFracture.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.7f, maxHealthRatio: 0f, system, ct);
    var temporalLeap = await GetSkillOrFailAsync("Temporal Leap", ct);
    await CreateAsync(temporalLeap.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var temporalShard = await GetSkillOrFailAsync("Temporal Shard", ct);
    await CreateAsync(temporalShard.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.95f, maxHealthRatio: 0f, system, ct);
    var hourglassReversal = await GetSkillOrFailAsync("Hourglass Reversal", ct);
    await CreateAsync(hourglassReversal.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // HERAKLES
    // =====================================================
    var titanicStrength = await GetSkillOrFailAsync("Titanic Strength", ct);
    await CreateAsync(titanicStrength.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var nemeanGrip = await GetSkillOrFailAsync("Nemean Grip", ct);
    await CreateAsync(nemeanGrip.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0.05f, system, ct);
    var lionsCharge = await GetSkillOrFailAsync("Lion's Charge", ct);
    await CreateAsync(lionsCharge.Id, attackDamageRatio: 0.4f, abilityPowerRatio: 0f, maxHealthRatio: 0.04f, system, ct);
    var lionsSkin = await GetSkillOrFailAsync("Lion's Skin", ct);
    await CreateAsync(lionsSkin.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.12f, system, ct);
    var twelveLabors = await GetSkillOrFailAsync("Twelve Labors", ct);
    await CreateAsync(twelveLabors.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.2f, system, ct);

    // =====================================================
    // ANUBIS
    // =====================================================
    var guardianoftheThreshold = await GetSkillOrFailAsync("Guardian of the Threshold", ct);
    await CreateAsync(guardianoftheThreshold.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.08f, system, ct);
    var judgmentsVeil = await GetSkillOrFailAsync("Judgment's Veil", ct);
    await CreateAsync(judgmentsVeil.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.4f, maxHealthRatio: 0.1f, system, ct);
    var passageofShadows = await GetSkillOrFailAsync("Passage of Shadows", ct);
    await CreateAsync(passageofShadows.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.2f, maxHealthRatio: 0.05f, system, ct);
    var funeraryScepter = await GetSkillOrFailAsync("Funerary Scepter", ct);
    await CreateAsync(funeraryScepter.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0.6f, maxHealthRatio: 0.03f, system, ct);
    var weighingofSouls = await GetSkillOrFailAsync("Weighing of Souls", ct);
    await CreateAsync(weighingofSouls.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0.15f, system, ct);

    // =====================================================
    // HEL
    // =====================================================
    var chilloftheDead = await GetSkillOrFailAsync("Chill of the Dead", ct);
    await CreateAsync(chilloftheDead.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var graspoftheFallen = await GetSkillOrFailAsync("Grasp of the Fallen", ct);
    await CreateAsync(graspoftheFallen.Id, attackDamageRatio: 0.3f, abilityPowerRatio: 0.3f, maxHealthRatio: 0.06f, system, ct);
    var walkoftheDead = await GetSkillOrFailAsync("Walk of the Dead", ct);
    await CreateAsync(walkoftheDead.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var icyBreath = await GetSkillOrFailAsync("Icy Breath", ct);
    await CreateAsync(icyBreath.Id, attackDamageRatio: 0.2f, abilityPowerRatio: 0.4f, maxHealthRatio: 0.04f, system, ct);
    var domainofHelheim = await GetSkillOrFailAsync("Domain of Helheim", ct);
    await CreateAsync(domainofHelheim.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0.1f, system, ct);

    // =====================================================
    // ISIS
    // =====================================================
    var ancientMagic = await GetSkillOrFailAsync("Ancient Magic", ct);
    await CreateAsync(ancientMagic.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var healingWing = await GetSkillOrFailAsync("Healing Wing", ct);
    await CreateAsync(healingWing.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.0f, maxHealthRatio: 0f, system, ct);
    var isissFlight = await GetSkillOrFailAsync("Isis's Flight", ct);
    await CreateAsync(isissFlight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.3f, maxHealthRatio: 0f, system, ct);
    var rayofLight = await GetSkillOrFailAsync("Ray of Light", ct);
    await CreateAsync(rayofLight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var osirissReprieve = await GetSkillOrFailAsync("Osiris's Reprieve", ct);
    await CreateAsync(osirissReprieve.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // FREYJA
    // =====================================================
    var valkyriesFavor = await GetSkillOrFailAsync("Valkyries' Favor", ct);
    await CreateAsync(valkyriesFavor.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var valkyriesBlessing = await GetSkillOrFailAsync("Valkyries' Blessing", ct);
    await CreateAsync(valkyriesBlessing.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.7f, maxHealthRatio: 0f, system, ct);
    var falconFlight = await GetSkillOrFailAsync("Falcon Flight", ct);
    await CreateAsync(falconFlight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.3f, maxHealthRatio: 0f, system, ct);
    var goldenBlade = await GetSkillOrFailAsync("Golden Blade", ct);
    await CreateAsync(goldenBlade.Id, attackDamageRatio: 0.3f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var fieldofFolkvangr = await GetSkillOrFailAsync("Field of Folkvangr", ct);
    await CreateAsync(fieldofFolkvangr.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ENKI
    // =====================================================
    var watersWisdom = await GetSkillOrFailAsync("Water's Wisdom", ct);
    await CreateAsync(watersWisdom.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var streamofWisdom = await GetSkillOrFailAsync("Stream of Wisdom", ct);
    await CreateAsync(streamofWisdom.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var undercurrent = await GetSkillOrFailAsync("Undercurrent", ct);
    await CreateAsync(undercurrent.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var invigoratingWave = await GetSkillOrFailAsync("Invigorating Wave", ct);
    await CreateAsync(invigoratingWave.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var abzu = await GetSkillOrFailAsync("Abzu", ct);
    await CreateAsync(abzu.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.9f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ARTEMIS
    // =====================================================
    var huntresssEye = await GetSkillOrFailAsync("Huntress's Eye", ct);
    await CreateAsync(huntresssEye.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var lunarArrow = await GetSkillOrFailAsync("Lunar Arrow", ct);
    await CreateAsync(lunarArrow.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var doesLeap = await GetSkillOrFailAsync("Doe's Leap", ct);
    await CreateAsync(doesLeap.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var rainofArrows = await GetSkillOrFailAsync("Rain of Arrows", ct);
    await CreateAsync(rainofArrows.Id, attackDamageRatio: 0.9f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var sacredHunt = await GetSkillOrFailAsync("Sacred Hunt", ct);
    await CreateAsync(sacredHunt.Id, attackDamageRatio: 1.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // VIDAR
    // =====================================================
    var avengingSilence = await GetSkillOrFailAsync("Avenging Silence", ct);
    await CreateAsync(avengingSilence.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var silencingShaft = await GetSkillOrFailAsync("Silencing Shaft", ct);
    await CreateAsync(silencingShaft.Id, attackDamageRatio: 1.3f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var ironStride = await GetSkillOrFailAsync("Iron Stride", ct);
    await CreateAsync(ironStride.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var crushingBoot = await GetSkillOrFailAsync("Crushing Boot", ct);
    await CreateAsync(crushingBoot.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var fenrirsVengeance = await GetSkillOrFailAsync("Fenrir's Vengeance", ct);
    await CreateAsync(fenrirsVengeance.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // RAMA
    // =====================================================
    var discipline = await GetSkillOrFailAsync("Discipline", ct);
    await CreateAsync(discipline.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var arrowofDharma = await GetSkillOrFailAsync("Arrow of Dharma", ct);
    await CreateAsync(arrowofDharma.Id, attackDamageRatio: 1.15f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var princesStride = await GetSkillOrFailAsync("Prince's Stride", ct);
    await CreateAsync(princesStride.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var chainedShot = await GetSkillOrFailAsync("Chained Shot", ct);
    await CreateAsync(chainedShot.Id, attackDamageRatio: 0.95f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var brahmastra = await GetSkillOrFailAsync("Brahmastra", ct);
    await CreateAsync(brahmastra.Id, attackDamageRatio: 1.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    await _uow.CommitAsync(ct);
  }

  private async Task CreateAsync(
      Guid skillId,
      float attackDamageRatio,
      float abilityPowerRatio,
      float maxHealthRatio,
      string createdBy,
      CancellationToken ct)
  {
    var profile = _factory.Create();
    profile.Define(skillId, attackDamageRatio, abilityPowerRatio, maxHealthRatio, createdBy);
    await _repo.AddAsync(profile, ct);
  }

  private async Task<Skill> GetSkillOrFailAsync(string name, CancellationToken ct) =>
      await _skills.GetByNameAsync(name, ct)
      ?? throw new InvalidOperationException($"Domain validation failed during seed. Missing Skill: {name}");
}