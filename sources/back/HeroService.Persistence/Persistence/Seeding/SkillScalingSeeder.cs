using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Structural AD/AP/MaxHealth scaling profile for all 150 skills. Not
/// version-scoped. Each ratio is the SUM of that stat's ratio across every
/// SkillEffect on the skill (see SkillSeeder) -- the skill's total scaling
/// exposure to that stat.
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
    var staticCharge = await GetSkillOrFailAsync("Static Charge", ct);
    await CreateAsync(staticCharge.Id, attackDamageRatio: 0.4f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var thunderStrike = await GetSkillOrFailAsync("Thunder Strike", ct);
    await CreateAsync(thunderStrike.Id, attackDamageRatio: 1.1f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var hammersCall = await GetSkillOrFailAsync("Hammer's Call", ct);
    await CreateAsync(hammersCall.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var mjolnirBlows = await GetSkillOrFailAsync("Mjolnir Blows", ct);
    await CreateAsync(mjolnirBlows.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var asgardsWrath = await GetSkillOrFailAsync("Asgard's Wrath", ct);
    await CreateAsync(asgardsWrath.Id, attackDamageRatio: 1.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ARES
    // =====================================================
    var bloodlust = await GetSkillOrFailAsync("Bloodlust", ct);
    await CreateAsync(bloodlust.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var bloodHarvest = await GetSkillOrFailAsync("Blood Harvest", ct);
    await CreateAsync(bloodHarvest.Id, attackDamageRatio: 1.3f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var butchersCharge = await GetSkillOrFailAsync("Butcher's Charge", ct);
    await CreateAsync(butchersCharge.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var spearBlows = await GetSkillOrFailAsync("Spear Blows", ct);
    await CreateAsync(spearBlows.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var warsFury = await GetSkillOrFailAsync("War's Fury", ct);
    await CreateAsync(warsFury.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // GUAN YU
    // =====================================================
    var unshakeableLoyalty = await GetSkillOrFailAsync("Unshakeable Loyalty", ct);
    await CreateAsync(unshakeableLoyalty.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.15f, system, ct);
    var greenDragonBlade = await GetSkillOrFailAsync("Green Dragon Blade", ct);
    await CreateAsync(greenDragonBlade.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var redHareCharge = await GetSkillOrFailAsync("Red Hare Charge", ct);
    await CreateAsync(redHareCharge.Id, attackDamageRatio: 0.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var halberdSweep = await GetSkillOrFailAsync("Halberd Sweep", ct);
    await CreateAsync(halberdSweep.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var oathoftheThreeBrothers = await GetSkillOrFailAsync("Oath of the Three Brothers", ct);
    await CreateAsync(oathoftheThreeBrothers.Id, attackDamageRatio: 1.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // OGUN
    // =====================================================
    var livingMetal = await GetSkillOrFailAsync("Living Metal", ct);
    await CreateAsync(livingMetal.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var ironEdge = await GetSkillOrFailAsync("Iron Edge", ct);
    await CreateAsync(ironEdge.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var blacksmithsStride = await GetSkillOrFailAsync("Blacksmith's Stride", ct);
    await CreateAsync(blacksmithsStride.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var macheteBlows = await GetSkillOrFailAsync("Machete Blows", ct);
    await CreateAsync(macheteBlows.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var forgesWrath = await GetSkillOrFailAsync("Forge's Wrath", ct);
    await CreateAsync(forgesWrath.Id, attackDamageRatio: 1.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // SUSANOO
    // =====================================================
    var breathofBattle = await GetSkillOrFailAsync("Breath of Battle", ct);
    await CreateAsync(breathofBattle.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var galeBlade = await GetSkillOrFailAsync("Gale Blade", ct);
    await CreateAsync(galeBlade.Id, attackDamageRatio: 0.95f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var typhoonStep = await GetSkillOrFailAsync("Typhoon Step", ct);
    await CreateAsync(typhoonStep.Id, attackDamageRatio: 0.75f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var kusanagiStrikes = await GetSkillOrFailAsync("Kusanagi Strikes", ct);
    await CreateAsync(kusanagiStrikes.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var orochi = await GetSkillOrFailAsync("Orochi", ct);
    await CreateAsync(orochi.Id, attackDamageRatio: 1.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // LOKI
    // =====================================================
    var deceit = await GetSkillOrFailAsync("Deceit", ct);
    await CreateAsync(deceit.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var twinBlades = await GetSkillOrFailAsync("Twin Blades", ct);
    await CreateAsync(twinBlades.Id, attackDamageRatio: 1.4000000000000001f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shadowFlee = await GetSkillOrFailAsync("Shadow Flee", ct);
    await CreateAsync(shadowFlee.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var sneakingDaggers = await GetSkillOrFailAsync("Sneaking Daggers", ct);
    await CreateAsync(sneakingDaggers.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var trickstersVerdict = await GetSkillOrFailAsync("Trickster's Verdict", ct);
    await CreateAsync(trickstersVerdict.Id, attackDamageRatio: 1.9f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // SET
    // =====================================================
    var desertsBlood = await GetSkillOrFailAsync("Desert's Blood", ct);
    await CreateAsync(desertsBlood.Id, attackDamageRatio: 0.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var sandstorm = await GetSkillOrFailAsync("Sandstorm", ct);
    await CreateAsync(sandstorm.Id, attackDamageRatio: 0.9f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var chaosMist = await GetSkillOrFailAsync("Chaos Mist", ct);
    await CreateAsync(chaosMist.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var khopeshBlows = await GetSkillOrFailAsync("Khopesh Blows", ct);
    await CreateAsync(khopeshBlows.Id, attackDamageRatio: 0.62f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var setsJudgment = await GetSkillOrFailAsync("Set's Judgment", ct);
    await CreateAsync(setsJudgment.Id, attackDamageRatio: 1.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // KALI
    // =====================================================
    var bloodRapture = await GetSkillOrFailAsync("Blood Rapture", ct);
    await CreateAsync(bloodRapture.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var danceofBlades = await GetSkillOrFailAsync("Dance of Blades", ct);
    await CreateAsync(danceofBlades.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var goddesssLeap = await GetSkillOrFailAsync("Goddess's Leap", ct);
    await CreateAsync(goddesssLeap.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var manyBlades = await GetSkillOrFailAsync("Many Blades", ct);
    await CreateAsync(manyBlades.Id, attackDamageRatio: 0.63f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var destructiveFury = await GetSkillOrFailAsync("Destructive Fury", ct);
    await CreateAsync(destructiveFury.Id, attackDamageRatio: 2.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // CAMAZOTZ
    // =====================================================
    var nocturnalFlight = await GetSkillOrFailAsync("Nocturnal Flight", ct);
    await CreateAsync(nocturnalFlight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var bloodSwarm = await GetSkillOrFailAsync("Blood Swarm", ct);
    await CreateAsync(bloodSwarm.Id, attackDamageRatio: 1.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var raptorDive = await GetSkillOrFailAsync("Raptor Dive", ct);
    await CreateAsync(raptorDive.Id, attackDamageRatio: 0.75f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var clawsandFangs = await GetSkillOrFailAsync("Claws and Fangs", ct);
    await CreateAsync(clawsandFangs.Id, attackDamageRatio: 0.61f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var feastofXibalba = await GetSkillOrFailAsync("Feast of Xibalba", ct);
    await CreateAsync(feastofXibalba.Id, attackDamageRatio: 2.4f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // NYX
    // =====================================================
    var nightfallVeil = await GetSkillOrFailAsync("Nightfall Veil", ct);
    await CreateAsync(nightfallVeil.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var bladeofDarkness = await GetSkillOrFailAsync("Blade of Darkness", ct);
    await CreateAsync(bladeofDarkness.Id, attackDamageRatio: 1.05f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shadowStep = await GetSkillOrFailAsync("Shadow Step", ct);
    await CreateAsync(shadowStep.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shadowBlades = await GetSkillOrFailAsync("Shadow Blades", ct);
    await CreateAsync(shadowBlades.Id, attackDamageRatio: 0.62f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var everlastingNight = await GetSkillOrFailAsync("Everlasting Night", ct);
    await CreateAsync(everlastingNight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ZEUS
    // =====================================================
    var celestialCharge = await GetSkillOrFailAsync("Celestial Charge", ct);
    await CreateAsync(celestialCharge.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var targetedBolt = await GetSkillOrFailAsync("Targeted Bolt", ct);
    await CreateAsync(targetedBolt.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.1f, maxHealthRatio: 0f, system, ct);
    var olympusGale = await GetSkillOrFailAsync("Olympus Gale", ct);
    await CreateAsync(olympusGale.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var sparks = await GetSkillOrFailAsync("Sparks", ct);
    await CreateAsync(sparks.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var skysWrath = await GetSkillOrFailAsync("Sky's Wrath", ct);
    await CreateAsync(skysWrath.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.6f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // RA
    // =====================================================
    var solarFire = await GetSkillOrFailAsync("Solar Fire", ct);
    await CreateAsync(solarFire.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.3f, maxHealthRatio: 0f, system, ct);
    var solarRay = await GetSkillOrFailAsync("Solar Ray", ct);
    await CreateAsync(solarRay.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.05f, maxHealthRatio: 0f, system, ct);
    var ascension = await GetSkillOrFailAsync("Ascension", ct);
    await CreateAsync(ascension.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var rays = await GetSkillOrFailAsync("Rays", ct);
    await CreateAsync(rays.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var noonJudgment = await GetSkillOrFailAsync("Noon Judgment", ct);
    await CreateAsync(noonJudgment.Id, attackDamageRatio: 0f, abilityPowerRatio: 2.7f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // AGNI
    // =====================================================
    var combustion = await GetSkillOrFailAsync("Combustion", ct);
    await CreateAsync(combustion.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var flameJavelin = await GetSkillOrFailAsync("Flame Javelin", ct);
    await CreateAsync(flameJavelin.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.05f, maxHealthRatio: 0f, system, ct);
    var blazingTrail = await GetSkillOrFailAsync("Blazing Trail", ct);
    await CreateAsync(blazingTrail.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var emberSpit = await GetSkillOrFailAsync("Ember Spit", ct);
    await CreateAsync(emberSpit.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var pillarofFire = await GetSkillOrFailAsync("Pillar of Fire", ct);
    await CreateAsync(pillarofFire.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.6f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // RAIJIN
    // =====================================================
    var stormsRhythm = await GetSkillOrFailAsync("Storm's Rhythm", ct);
    await CreateAsync(stormsRhythm.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var thunderclap = await GetSkillOrFailAsync("Thunderclap", ct);
    await CreateAsync(thunderclap.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.05f, maxHealthRatio: 0f, system, ct);
    var thunderStep = await GetSkillOrFailAsync("Thunder Step", ct);
    await CreateAsync(thunderStep.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var drumRolls = await GetSkillOrFailAsync("Drum Rolls", ct);
    await CreateAsync(drumRolls.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var drumFury = await GetSkillOrFailAsync("Drum Fury", ct);
    await CreateAsync(drumFury.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.6f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // MARDUK
    // =====================================================
    var tabletsofDestiny = await GetSkillOrFailAsync("Tablets of Destiny", ct);
    await CreateAsync(tabletsofDestiny.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var netoftheWinds = await GetSkillOrFailAsync("Net of the Winds", ct);
    await CreateAsync(netoftheWinds.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.0f, maxHealthRatio: 0f, system, ct);
    var primordialBreath = await GetSkillOrFailAsync("Primordial Breath", ct);
    await CreateAsync(primordialBreath.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var shardsofPower = await GetSkillOrFailAsync("Shards of Power", ct);
    await CreateAsync(shardsofPower.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var sealofTiamat = await GetSkillOrFailAsync("Seal of Tiamat", ct);
    await CreateAsync(sealofTiamat.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.5f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // HERAKLES
    // =====================================================
    var herosEndurance = await GetSkillOrFailAsync("Hero's Endurance", ct);
    await CreateAsync(herosEndurance.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var nemeanGrip = await GetSkillOrFailAsync("Nemean Grip", ct);
    await CreateAsync(nemeanGrip.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0.05f, system, ct);
    var lionsCharge = await GetSkillOrFailAsync("Lion's Charge", ct);
    await CreateAsync(lionsCharge.Id, attackDamageRatio: 0.4f, abilityPowerRatio: 0f, maxHealthRatio: 0.04f, system, ct);
    var clubBlows = await GetSkillOrFailAsync("Club Blows", ct);
    await CreateAsync(clubBlows.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var twelveLabors = await GetSkillOrFailAsync("Twelve Labors", ct);
    await CreateAsync(twelveLabors.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // YMIR
    // =====================================================
    var fleshofIce = await GetSkillOrFailAsync("Flesh of Ice", ct);
    await CreateAsync(fleshofIce.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.02f, system, ct);
    var glacialShard = await GetSkillOrFailAsync("Glacial Shard", ct);
    await CreateAsync(glacialShard.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var wallofFrost = await GetSkillOrFailAsync("Wall of Frost", ct);
    await CreateAsync(wallofFrost.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var frostFists = await GetSkillOrFailAsync("Frost Fists", ct);
    await CreateAsync(frostFists.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var gripofFrost = await GetSkillOrFailAsync("Grip of Frost", ct);
    await CreateAsync(gripofFrost.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // GEB
    // =====================================================
    var stoneSkin = await GetSkillOrFailAsync("Stone Skin", ct);
    await CreateAsync(stoneSkin.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var telluricShard = await GetSkillOrFailAsync("Telluric Shard", ct);
    await CreateAsync(telluricShard.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var earthenAegis = await GetSkillOrFailAsync("Earthen Aegis", ct);
    await CreateAsync(earthenAegis.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0.06f, system, ct);
    var stoneFists = await GetSkillOrFailAsync("Stone Fists", ct);
    await CreateAsync(stoneFists.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var cataclysm = await GetSkillOrFailAsync("Cataclysm", ct);
    await CreateAsync(cataclysm.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // KUMBHAKARNA
    // =====================================================
    var giantsSlumber = await GetSkillOrFailAsync("Giant's Slumber", ct);
    await CreateAsync(giantsSlumber.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var greatSweep = await GetSkillOrFailAsync("Great Sweep", ct);
    await CreateAsync(greatSweep.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var ponderousStride = await GetSkillOrFailAsync("Ponderous Stride", ct);
    await CreateAsync(ponderousStride.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var massiveBackhand = await GetSkillOrFailAsync("Massive Backhand", ct);
    await CreateAsync(massiveBackhand.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var terribleAwakening = await GetSkillOrFailAsync("Terrible Awakening", ct);
    await CreateAsync(terribleAwakening.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // GILGAMESH
    // =====================================================
    var twoThirdsDivine = await GetSkillOrFailAsync("Two-Thirds Divine", ct);
    await CreateAsync(twoThirdsDivine.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var celestialSlash = await GetSkillOrFailAsync("Celestial Slash", ct);
    await CreateAsync(celestialSlash.Id, attackDamageRatio: 0.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var chargeofUruk = await GetSkillOrFailAsync("Charge of Uruk", ct);
    await CreateAsync(chargeofUruk.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var royalBlows = await GetSkillOrFailAsync("Royal Blows", ct);
    await CreateAsync(royalBlows.Id, attackDamageRatio: 0.55f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var kingsJudgment = await GetSkillOrFailAsync("King's Judgment", ct);
    await CreateAsync(kingsJudgment.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // FREYJA
    // =====================================================
    var vanirsFavor = await GetSkillOrFailAsync("Vanir's Favor", ct);
    await CreateAsync(vanirsFavor.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var valkyriesBlessing = await GetSkillOrFailAsync("Valkyries' Blessing", ct);
    await CreateAsync(valkyriesBlessing.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var falconFlight = await GetSkillOrFailAsync("Falcon Flight", ct);
    await CreateAsync(falconFlight.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var goldenShards = await GetSkillOrFailAsync("Golden Shards", ct);
    await CreateAsync(goldenShards.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var dawnofFolkvangr = await GetSkillOrFailAsync("Dawn of Folkvangr", ct);
    await CreateAsync(dawnofFolkvangr.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.4f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ISIS
    // =====================================================
    var ancestralMagic = await GetSkillOrFailAsync("Ancestral Magic", ct);
    await CreateAsync(ancestralMagic.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var healingWing = await GetSkillOrFailAsync("Healing Wing", ct);
    await CreateAsync(healingWing.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var veilofIsis = await GetSkillOrFailAsync("Veil of Isis", ct);
    await CreateAsync(veilofIsis.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0f, system, ct);
    var breathofLife = await GetSkillOrFailAsync("Breath of Life", ct);
    await CreateAsync(breathofLife.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var osirissResurrection = await GetSkillOrFailAsync("Osiris's Resurrection", ct);
    await CreateAsync(osirissResurrection.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.3f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // APHRODITE
    // =====================================================
    var grace = await GetSkillOrFailAsync("Grace", ct);
    await CreateAsync(grace.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var embrace = await GetSkillOrFailAsync("Embrace", ct);
    await CreateAsync(embrace.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var flightofDoves = await GetSkillOrFailAsync("Flight of Doves", ct);
    await CreateAsync(flightofDoves.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var ardentKisses = await GetSkillOrFailAsync("Ardent Kisses", ct);
    await CreateAsync(ardentKisses.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var intoxicatingCharm = await GetSkillOrFailAsync("Intoxicating Charm", ct);
    await CreateAsync(intoxicatingCharm.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // GUANYIN
    // =====================================================
    var compassion = await GetSkillOrFailAsync("Compassion", ct);
    await CreateAsync(compassion.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var willowWater = await GetSkillOrFailAsync("Willow Water", ct);
    await CreateAsync(willowWater.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var lotusStep = await GetSkillOrFailAsync("Lotus Step", ct);
    await CreateAsync(lotusStep.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var jadeDroplets = await GetSkillOrFailAsync("Jade Droplets", ct);
    await CreateAsync(jadeDroplets.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var oceanofMercy = await GetSkillOrFailAsync("Ocean of Mercy", ct);
    await CreateAsync(oceanofMercy.Id, attackDamageRatio: 0f, abilityPowerRatio: 1.4f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // BRIGID
    // =====================================================
    var eternalFlame = await GetSkillOrFailAsync("Eternal Flame", ct);
    await CreateAsync(eternalFlame.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.4f, maxHealthRatio: 0f, system, ct);
    var forgeShield = await GetSkillOrFailAsync("Forge Shield", ct);
    await CreateAsync(forgeShield.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.8f, maxHealthRatio: 0f, system, ct);
    var emberBreath = await GetSkillOrFailAsync("Ember Breath", ct);
    await CreateAsync(emberBreath.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var sacredEmbers = await GetSkillOrFailAsync("Sacred Embers", ct);
    await CreateAsync(sacredEmbers.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.5f, maxHealthRatio: 0f, system, ct);
    var inspiringBlaze = await GetSkillOrFailAsync("Inspiring Blaze", ct);
    await CreateAsync(inspiringBlaze.Id, attackDamageRatio: 0f, abilityPowerRatio: 0.9f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ARTEMIS
    // =====================================================
    var huntresssEye = await GetSkillOrFailAsync("Huntress's Eye", ct);
    await CreateAsync(huntresssEye.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var piercingArrow = await GetSkillOrFailAsync("Piercing Arrow", ct);
    await CreateAsync(piercingArrow.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var doesLeap = await GetSkillOrFailAsync("Doe's Leap", ct);
    await CreateAsync(doesLeap.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var lunarShots = await GetSkillOrFailAsync("Lunar Shots", ct);
    await CreateAsync(lunarShots.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var arrowoftheMoon = await GetSkillOrFailAsync("Arrow of the Moon", ct);
    await CreateAsync(arrowoftheMoon.Id, attackDamageRatio: 2.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // RAMA
    // =====================================================
    var princesPrecision = await GetSkillOrFailAsync("Prince's Precision", ct);
    await CreateAsync(princesPrecision.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var blazingShaft = await GetSkillOrFailAsync("Blazing Shaft", ct);
    await CreateAsync(blazingShaft.Id, attackDamageRatio: 1.5f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var princesStep = await GetSkillOrFailAsync("Prince's Step", ct);
    await CreateAsync(princesStep.Id, attackDamageRatio: 0.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var arrowsofKodanda = await GetSkillOrFailAsync("Arrows of Kodanda", ct);
    await CreateAsync(arrowsofKodanda.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var brahmastra = await GetSkillOrFailAsync("Brahmastra", ct);
    await CreateAsync(brahmastra.Id, attackDamageRatio: 2.6f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // HOUYI
    // =====================================================
    var nineSuns = await GetSkillOrFailAsync("Nine Suns", ct);
    await CreateAsync(nineSuns.Id, attackDamageRatio: 0.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var solarArrow = await GetSkillOrFailAsync("Solar Arrow", ct);
    await CreateAsync(solarArrow.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var huntersRoll = await GetSkillOrFailAsync("Hunter's Roll", ct);
    await CreateAsync(huntersRoll.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var burningArrows = await GetSkillOrFailAsync("Burning Arrows", ct);
    await CreateAsync(burningArrows.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var volleyofTenSuns = await GetSkillOrFailAsync("Volley of Ten Suns", ct);
    await CreateAsync(volleyofTenSuns.Id, attackDamageRatio: 2.0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // ULLR
    // =====================================================
    var wintersFavor = await GetSkillOrFailAsync("Winter's Favor", ct);
    await CreateAsync(wintersFavor.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var piercingShaft = await GetSkillOrFailAsync("Piercing Shaft", ct);
    await CreateAsync(piercingShaft.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var icyGlide = await GetSkillOrFailAsync("Icy Glide", ct);
    await CreateAsync(icyGlide.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var yewArrows = await GetSkillOrFailAsync("Yew Arrows", ct);
    await CreateAsync(yewArrows.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var duelistsChallenge = await GetSkillOrFailAsync("Duelist's Challenge", ct);
    await CreateAsync(duelistsChallenge.Id, attackDamageRatio: 1.7f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

    // =====================================================
    // NEITH
    // =====================================================
    var threadofFate = await GetSkillOrFailAsync("Thread of Fate", ct);
    await CreateAsync(threadofFate.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var huntingShaft = await GetSkillOrFailAsync("Hunting Shaft", ct);
    await CreateAsync(huntingShaft.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var weaversStep = await GetSkillOrFailAsync("Weaver's Step", ct);
    await CreateAsync(weaversStep.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var wovenArrows = await GetSkillOrFailAsync("Woven Arrows", ct);
    await CreateAsync(wovenArrows.Id, attackDamageRatio: 0.65f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);
    var weboftheWorld = await GetSkillOrFailAsync("Web of the World", ct);
    await CreateAsync(weboftheWorld.Id, attackDamageRatio: 0f, abilityPowerRatio: 0f, maxHealthRatio: 0f, system, ct);

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