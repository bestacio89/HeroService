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
/// Version-scoped balance multipliers for all 150 skills, seeded against
/// GameVersion 1.0.0. Neutral (1.0f) -- see HeroModifierSeeder remarks for
/// why: no real balance-patch delta exists yet.
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
    var staticCharge = await GetSkill("Static Charge", ct);
    var thunderStrike = await GetSkill("Thunder Strike", ct);
    var hammersCall = await GetSkill("Hammer's Call", ct);
    var mjolnirBlows = await GetSkill("Mjolnir Blows", ct);
    var asgardsWrath = await GetSkill("Asgard's Wrath", ct);

    // =====================================================
    // ARES
    // =====================================================
    var bloodlust = await GetSkill("Bloodlust", ct);
    var bloodHarvest = await GetSkill("Blood Harvest", ct);
    var butchersCharge = await GetSkill("Butcher's Charge", ct);
    var spearBlows = await GetSkill("Spear Blows", ct);
    var warsFury = await GetSkill("War's Fury", ct);

    // =====================================================
    // GUAN YU
    // =====================================================
    var unshakeableLoyalty = await GetSkill("Unshakeable Loyalty", ct);
    var greenDragonBlade = await GetSkill("Green Dragon Blade", ct);
    var redHareCharge = await GetSkill("Red Hare Charge", ct);
    var halberdSweep = await GetSkill("Halberd Sweep", ct);
    var oathoftheThreeBrothers = await GetSkill("Oath of the Three Brothers", ct);

    // =====================================================
    // OGUN
    // =====================================================
    var livingMetal = await GetSkill("Living Metal", ct);
    var ironEdge = await GetSkill("Iron Edge", ct);
    var blacksmithsStride = await GetSkill("Blacksmith's Stride", ct);
    var macheteBlows = await GetSkill("Machete Blows", ct);
    var forgesWrath = await GetSkill("Forge's Wrath", ct);

    // =====================================================
    // SUSANOO
    // =====================================================
    var breathofBattle = await GetSkill("Breath of Battle", ct);
    var galeBlade = await GetSkill("Gale Blade", ct);
    var typhoonStep = await GetSkill("Typhoon Step", ct);
    var kusanagiStrikes = await GetSkill("Kusanagi Strikes", ct);
    var orochi = await GetSkill("Orochi", ct);

    // =====================================================
    // LOKI
    // =====================================================
    var deceit = await GetSkill("Deceit", ct);
    var twinBlades = await GetSkill("Twin Blades", ct);
    var shadowFlee = await GetSkill("Shadow Flee", ct);
    var sneakingDaggers = await GetSkill("Sneaking Daggers", ct);
    var trickstersVerdict = await GetSkill("Trickster's Verdict", ct);

    // =====================================================
    // SET
    // =====================================================
    var desertsBlood = await GetSkill("Desert's Blood", ct);
    var sandstorm = await GetSkill("Sandstorm", ct);
    var chaosMist = await GetSkill("Chaos Mist", ct);
    var khopeshBlows = await GetSkill("Khopesh Blows", ct);
    var setsJudgment = await GetSkill("Set's Judgment", ct);

    // =====================================================
    // KALI
    // =====================================================
    var bloodRapture = await GetSkill("Blood Rapture", ct);
    var danceofBlades = await GetSkill("Dance of Blades", ct);
    var goddesssLeap = await GetSkill("Goddess's Leap", ct);
    var manyBlades = await GetSkill("Many Blades", ct);
    var destructiveFury = await GetSkill("Destructive Fury", ct);

    // =====================================================
    // CAMAZOTZ
    // =====================================================
    var nocturnalFlight = await GetSkill("Nocturnal Flight", ct);
    var bloodSwarm = await GetSkill("Blood Swarm", ct);
    var raptorDive = await GetSkill("Raptor Dive", ct);
    var clawsandFangs = await GetSkill("Claws and Fangs", ct);
    var feastofXibalba = await GetSkill("Feast of Xibalba", ct);

    // =====================================================
    // NYX
    // =====================================================
    var nightfallVeil = await GetSkill("Nightfall Veil", ct);
    var bladeofDarkness = await GetSkill("Blade of Darkness", ct);
    var shadowStep = await GetSkill("Shadow Step", ct);
    var shadowBlades = await GetSkill("Shadow Blades", ct);
    var everlastingNight = await GetSkill("Everlasting Night", ct);

    // =====================================================
    // ZEUS
    // =====================================================
    var celestialCharge = await GetSkill("Celestial Charge", ct);
    var targetedBolt = await GetSkill("Targeted Bolt", ct);
    var olympusGale = await GetSkill("Olympus Gale", ct);
    var sparks = await GetSkill("Sparks", ct);
    var skysWrath = await GetSkill("Sky's Wrath", ct);

    // =====================================================
    // RA
    // =====================================================
    var solarFire = await GetSkill("Solar Fire", ct);
    var solarRay = await GetSkill("Solar Ray", ct);
    var ascension = await GetSkill("Ascension", ct);
    var rays = await GetSkill("Rays", ct);
    var noonJudgment = await GetSkill("Noon Judgment", ct);

    // =====================================================
    // AGNI
    // =====================================================
    var combustion = await GetSkill("Combustion", ct);
    var flameJavelin = await GetSkill("Flame Javelin", ct);
    var blazingTrail = await GetSkill("Blazing Trail", ct);
    var emberSpit = await GetSkill("Ember Spit", ct);
    var pillarofFire = await GetSkill("Pillar of Fire", ct);

    // =====================================================
    // RAIJIN
    // =====================================================
    var stormsRhythm = await GetSkill("Storm's Rhythm", ct);
    var thunderclap = await GetSkill("Thunderclap", ct);
    var thunderStep = await GetSkill("Thunder Step", ct);
    var drumRolls = await GetSkill("Drum Rolls", ct);
    var drumFury = await GetSkill("Drum Fury", ct);

    // =====================================================
    // MARDUK
    // =====================================================
    var tabletsofDestiny = await GetSkill("Tablets of Destiny", ct);
    var netoftheWinds = await GetSkill("Net of the Winds", ct);
    var primordialBreath = await GetSkill("Primordial Breath", ct);
    var shardsofPower = await GetSkill("Shards of Power", ct);
    var sealofTiamat = await GetSkill("Seal of Tiamat", ct);

    // =====================================================
    // HERAKLES
    // =====================================================
    var herosEndurance = await GetSkill("Hero's Endurance", ct);
    var nemeanGrip = await GetSkill("Nemean Grip", ct);
    var lionsCharge = await GetSkill("Lion's Charge", ct);
    var clubBlows = await GetSkill("Club Blows", ct);
    var twelveLabors = await GetSkill("Twelve Labors", ct);

    // =====================================================
    // YMIR
    // =====================================================
    var fleshofIce = await GetSkill("Flesh of Ice", ct);
    var glacialShard = await GetSkill("Glacial Shard", ct);
    var wallofFrost = await GetSkill("Wall of Frost", ct);
    var frostFists = await GetSkill("Frost Fists", ct);
    var gripofFrost = await GetSkill("Grip of Frost", ct);

    // =====================================================
    // GEB
    // =====================================================
    var stoneSkin = await GetSkill("Stone Skin", ct);
    var telluricShard = await GetSkill("Telluric Shard", ct);
    var earthenAegis = await GetSkill("Earthen Aegis", ct);
    var stoneFists = await GetSkill("Stone Fists", ct);
    var cataclysm = await GetSkill("Cataclysm", ct);

    // =====================================================
    // KUMBHAKARNA
    // =====================================================
    var giantsSlumber = await GetSkill("Giant's Slumber", ct);
    var greatSweep = await GetSkill("Great Sweep", ct);
    var ponderousStride = await GetSkill("Ponderous Stride", ct);
    var massiveBackhand = await GetSkill("Massive Backhand", ct);
    var terribleAwakening = await GetSkill("Terrible Awakening", ct);

    // =====================================================
    // GILGAMESH
    // =====================================================
    var twoThirdsDivine = await GetSkill("Two-Thirds Divine", ct);
    var celestialSlash = await GetSkill("Celestial Slash", ct);
    var chargeofUruk = await GetSkill("Charge of Uruk", ct);
    var royalBlows = await GetSkill("Royal Blows", ct);
    var kingsJudgment = await GetSkill("King's Judgment", ct);

    // =====================================================
    // FREYJA
    // =====================================================
    var vanirsFavor = await GetSkill("Vanir's Favor", ct);
    var valkyriesBlessing = await GetSkill("Valkyries' Blessing", ct);
    var falconFlight = await GetSkill("Falcon Flight", ct);
    var goldenShards = await GetSkill("Golden Shards", ct);
    var dawnofFolkvangr = await GetSkill("Dawn of Folkvangr", ct);

    // =====================================================
    // ISIS
    // =====================================================
    var ancestralMagic = await GetSkill("Ancestral Magic", ct);
    var healingWing = await GetSkill("Healing Wing", ct);
    var veilofIsis = await GetSkill("Veil of Isis", ct);
    var breathofLife = await GetSkill("Breath of Life", ct);
    var osirissResurrection = await GetSkill("Osiris's Resurrection", ct);

    // =====================================================
    // APHRODITE
    // =====================================================
    var grace = await GetSkill("Grace", ct);
    var embrace = await GetSkill("Embrace", ct);
    var flightofDoves = await GetSkill("Flight of Doves", ct);
    var ardentKisses = await GetSkill("Ardent Kisses", ct);
    var intoxicatingCharm = await GetSkill("Intoxicating Charm", ct);

    // =====================================================
    // GUANYIN
    // =====================================================
    var compassion = await GetSkill("Compassion", ct);
    var willowWater = await GetSkill("Willow Water", ct);
    var lotusStep = await GetSkill("Lotus Step", ct);
    var jadeDroplets = await GetSkill("Jade Droplets", ct);
    var oceanofMercy = await GetSkill("Ocean of Mercy", ct);

    // =====================================================
    // BRIGID
    // =====================================================
    var eternalFlame = await GetSkill("Eternal Flame", ct);
    var forgeShield = await GetSkill("Forge Shield", ct);
    var emberBreath = await GetSkill("Ember Breath", ct);
    var sacredEmbers = await GetSkill("Sacred Embers", ct);
    var inspiringBlaze = await GetSkill("Inspiring Blaze", ct);

    // =====================================================
    // ARTEMIS
    // =====================================================
    var huntresssEye = await GetSkill("Huntress's Eye", ct);
    var piercingArrow = await GetSkill("Piercing Arrow", ct);
    var doesLeap = await GetSkill("Doe's Leap", ct);
    var lunarShots = await GetSkill("Lunar Shots", ct);
    var arrowoftheMoon = await GetSkill("Arrow of the Moon", ct);

    // =====================================================
    // RAMA
    // =====================================================
    var princesPrecision = await GetSkill("Prince's Precision", ct);
    var blazingShaft = await GetSkill("Blazing Shaft", ct);
    var princesStep = await GetSkill("Prince's Step", ct);
    var arrowsofKodanda = await GetSkill("Arrows of Kodanda", ct);
    var brahmastra = await GetSkill("Brahmastra", ct);

    // =====================================================
    // HOUYI
    // =====================================================
    var nineSuns = await GetSkill("Nine Suns", ct);
    var solarArrow = await GetSkill("Solar Arrow", ct);
    var huntersRoll = await GetSkill("Hunter's Roll", ct);
    var burningArrows = await GetSkill("Burning Arrows", ct);
    var volleyofTenSuns = await GetSkill("Volley of Ten Suns", ct);

    // =====================================================
    // ULLR
    // =====================================================
    var wintersFavor = await GetSkill("Winter's Favor", ct);
    var piercingShaft = await GetSkill("Piercing Shaft", ct);
    var icyGlide = await GetSkill("Icy Glide", ct);
    var yewArrows = await GetSkill("Yew Arrows", ct);
    var duelistsChallenge = await GetSkill("Duelist's Challenge", ct);

    // =====================================================
    // NEITH
    // =====================================================
    var threadofFate = await GetSkill("Thread of Fate", ct);
    var huntingShaft = await GetSkill("Hunting Shaft", ct);
    var weaversStep = await GetSkill("Weaver's Step", ct);
    var wovenArrows = await GetSkill("Woven Arrows", ct);
    var weboftheWorld = await GetSkill("Web of the World", ct);

    // Neutral (1.0f) multipliers for every skill -- see class remarks.
    await CreateAsync(version.Id, staticCharge.Id, ct);
    await CreateAsync(version.Id, thunderStrike.Id, ct);
    await CreateAsync(version.Id, hammersCall.Id, ct);
    await CreateAsync(version.Id, mjolnirBlows.Id, ct);
    await CreateAsync(version.Id, asgardsWrath.Id, ct);
    await CreateAsync(version.Id, bloodlust.Id, ct);
    await CreateAsync(version.Id, bloodHarvest.Id, ct);
    await CreateAsync(version.Id, butchersCharge.Id, ct);
    await CreateAsync(version.Id, spearBlows.Id, ct);
    await CreateAsync(version.Id, warsFury.Id, ct);
    await CreateAsync(version.Id, unshakeableLoyalty.Id, ct);
    await CreateAsync(version.Id, greenDragonBlade.Id, ct);
    await CreateAsync(version.Id, redHareCharge.Id, ct);
    await CreateAsync(version.Id, halberdSweep.Id, ct);
    await CreateAsync(version.Id, oathoftheThreeBrothers.Id, ct);
    await CreateAsync(version.Id, livingMetal.Id, ct);
    await CreateAsync(version.Id, ironEdge.Id, ct);
    await CreateAsync(version.Id, blacksmithsStride.Id, ct);
    await CreateAsync(version.Id, macheteBlows.Id, ct);
    await CreateAsync(version.Id, forgesWrath.Id, ct);
    await CreateAsync(version.Id, breathofBattle.Id, ct);
    await CreateAsync(version.Id, galeBlade.Id, ct);
    await CreateAsync(version.Id, typhoonStep.Id, ct);
    await CreateAsync(version.Id, kusanagiStrikes.Id, ct);
    await CreateAsync(version.Id, orochi.Id, ct);
    await CreateAsync(version.Id, deceit.Id, ct);
    await CreateAsync(version.Id, twinBlades.Id, ct);
    await CreateAsync(version.Id, shadowFlee.Id, ct);
    await CreateAsync(version.Id, sneakingDaggers.Id, ct);
    await CreateAsync(version.Id, trickstersVerdict.Id, ct);
    await CreateAsync(version.Id, desertsBlood.Id, ct);
    await CreateAsync(version.Id, sandstorm.Id, ct);
    await CreateAsync(version.Id, chaosMist.Id, ct);
    await CreateAsync(version.Id, khopeshBlows.Id, ct);
    await CreateAsync(version.Id, setsJudgment.Id, ct);
    await CreateAsync(version.Id, bloodRapture.Id, ct);
    await CreateAsync(version.Id, danceofBlades.Id, ct);
    await CreateAsync(version.Id, goddesssLeap.Id, ct);
    await CreateAsync(version.Id, manyBlades.Id, ct);
    await CreateAsync(version.Id, destructiveFury.Id, ct);
    await CreateAsync(version.Id, nocturnalFlight.Id, ct);
    await CreateAsync(version.Id, bloodSwarm.Id, ct);
    await CreateAsync(version.Id, raptorDive.Id, ct);
    await CreateAsync(version.Id, clawsandFangs.Id, ct);
    await CreateAsync(version.Id, feastofXibalba.Id, ct);
    await CreateAsync(version.Id, nightfallVeil.Id, ct);
    await CreateAsync(version.Id, bladeofDarkness.Id, ct);
    await CreateAsync(version.Id, shadowStep.Id, ct);
    await CreateAsync(version.Id, shadowBlades.Id, ct);
    await CreateAsync(version.Id, everlastingNight.Id, ct);
    await CreateAsync(version.Id, celestialCharge.Id, ct);
    await CreateAsync(version.Id, targetedBolt.Id, ct);
    await CreateAsync(version.Id, olympusGale.Id, ct);
    await CreateAsync(version.Id, sparks.Id, ct);
    await CreateAsync(version.Id, skysWrath.Id, ct);
    await CreateAsync(version.Id, solarFire.Id, ct);
    await CreateAsync(version.Id, solarRay.Id, ct);
    await CreateAsync(version.Id, ascension.Id, ct);
    await CreateAsync(version.Id, rays.Id, ct);
    await CreateAsync(version.Id, noonJudgment.Id, ct);
    await CreateAsync(version.Id, combustion.Id, ct);
    await CreateAsync(version.Id, flameJavelin.Id, ct);
    await CreateAsync(version.Id, blazingTrail.Id, ct);
    await CreateAsync(version.Id, emberSpit.Id, ct);
    await CreateAsync(version.Id, pillarofFire.Id, ct);
    await CreateAsync(version.Id, stormsRhythm.Id, ct);
    await CreateAsync(version.Id, thunderclap.Id, ct);
    await CreateAsync(version.Id, thunderStep.Id, ct);
    await CreateAsync(version.Id, drumRolls.Id, ct);
    await CreateAsync(version.Id, drumFury.Id, ct);
    await CreateAsync(version.Id, tabletsofDestiny.Id, ct);
    await CreateAsync(version.Id, netoftheWinds.Id, ct);
    await CreateAsync(version.Id, primordialBreath.Id, ct);
    await CreateAsync(version.Id, shardsofPower.Id, ct);
    await CreateAsync(version.Id, sealofTiamat.Id, ct);
    await CreateAsync(version.Id, herosEndurance.Id, ct);
    await CreateAsync(version.Id, nemeanGrip.Id, ct);
    await CreateAsync(version.Id, lionsCharge.Id, ct);
    await CreateAsync(version.Id, clubBlows.Id, ct);
    await CreateAsync(version.Id, twelveLabors.Id, ct);
    await CreateAsync(version.Id, fleshofIce.Id, ct);
    await CreateAsync(version.Id, glacialShard.Id, ct);
    await CreateAsync(version.Id, wallofFrost.Id, ct);
    await CreateAsync(version.Id, frostFists.Id, ct);
    await CreateAsync(version.Id, gripofFrost.Id, ct);
    await CreateAsync(version.Id, stoneSkin.Id, ct);
    await CreateAsync(version.Id, telluricShard.Id, ct);
    await CreateAsync(version.Id, earthenAegis.Id, ct);
    await CreateAsync(version.Id, stoneFists.Id, ct);
    await CreateAsync(version.Id, cataclysm.Id, ct);
    await CreateAsync(version.Id, giantsSlumber.Id, ct);
    await CreateAsync(version.Id, greatSweep.Id, ct);
    await CreateAsync(version.Id, ponderousStride.Id, ct);
    await CreateAsync(version.Id, massiveBackhand.Id, ct);
    await CreateAsync(version.Id, terribleAwakening.Id, ct);
    await CreateAsync(version.Id, twoThirdsDivine.Id, ct);
    await CreateAsync(version.Id, celestialSlash.Id, ct);
    await CreateAsync(version.Id, chargeofUruk.Id, ct);
    await CreateAsync(version.Id, royalBlows.Id, ct);
    await CreateAsync(version.Id, kingsJudgment.Id, ct);
    await CreateAsync(version.Id, vanirsFavor.Id, ct);
    await CreateAsync(version.Id, valkyriesBlessing.Id, ct);
    await CreateAsync(version.Id, falconFlight.Id, ct);
    await CreateAsync(version.Id, goldenShards.Id, ct);
    await CreateAsync(version.Id, dawnofFolkvangr.Id, ct);
    await CreateAsync(version.Id, ancestralMagic.Id, ct);
    await CreateAsync(version.Id, healingWing.Id, ct);
    await CreateAsync(version.Id, veilofIsis.Id, ct);
    await CreateAsync(version.Id, breathofLife.Id, ct);
    await CreateAsync(version.Id, osirissResurrection.Id, ct);
    await CreateAsync(version.Id, grace.Id, ct);
    await CreateAsync(version.Id, embrace.Id, ct);
    await CreateAsync(version.Id, flightofDoves.Id, ct);
    await CreateAsync(version.Id, ardentKisses.Id, ct);
    await CreateAsync(version.Id, intoxicatingCharm.Id, ct);
    await CreateAsync(version.Id, compassion.Id, ct);
    await CreateAsync(version.Id, willowWater.Id, ct);
    await CreateAsync(version.Id, lotusStep.Id, ct);
    await CreateAsync(version.Id, jadeDroplets.Id, ct);
    await CreateAsync(version.Id, oceanofMercy.Id, ct);
    await CreateAsync(version.Id, eternalFlame.Id, ct);
    await CreateAsync(version.Id, forgeShield.Id, ct);
    await CreateAsync(version.Id, emberBreath.Id, ct);
    await CreateAsync(version.Id, sacredEmbers.Id, ct);
    await CreateAsync(version.Id, inspiringBlaze.Id, ct);
    await CreateAsync(version.Id, huntresssEye.Id, ct);
    await CreateAsync(version.Id, piercingArrow.Id, ct);
    await CreateAsync(version.Id, doesLeap.Id, ct);
    await CreateAsync(version.Id, lunarShots.Id, ct);
    await CreateAsync(version.Id, arrowoftheMoon.Id, ct);
    await CreateAsync(version.Id, princesPrecision.Id, ct);
    await CreateAsync(version.Id, blazingShaft.Id, ct);
    await CreateAsync(version.Id, princesStep.Id, ct);
    await CreateAsync(version.Id, arrowsofKodanda.Id, ct);
    await CreateAsync(version.Id, brahmastra.Id, ct);
    await CreateAsync(version.Id, nineSuns.Id, ct);
    await CreateAsync(version.Id, solarArrow.Id, ct);
    await CreateAsync(version.Id, huntersRoll.Id, ct);
    await CreateAsync(version.Id, burningArrows.Id, ct);
    await CreateAsync(version.Id, volleyofTenSuns.Id, ct);
    await CreateAsync(version.Id, wintersFavor.Id, ct);
    await CreateAsync(version.Id, piercingShaft.Id, ct);
    await CreateAsync(version.Id, icyGlide.Id, ct);
    await CreateAsync(version.Id, yewArrows.Id, ct);
    await CreateAsync(version.Id, duelistsChallenge.Id, ct);
    await CreateAsync(version.Id, threadofFate.Id, ct);
    await CreateAsync(version.Id, huntingShaft.Id, ct);
    await CreateAsync(version.Id, weaversStep.Id, ct);
    await CreateAsync(version.Id, wovenArrows.Id, ct);
    await CreateAsync(version.Id, weboftheWorld.Id, ct);

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