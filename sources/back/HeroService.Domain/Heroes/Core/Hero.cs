using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Core.Skills;

namespace HeroService.Domain.Heroes.Core;

/// <summary>
/// Identity-only aggregate root.
/// 
/// Domain Role:
/// Hero is the central gameplay entity representing a fully defined combat unit.
/// It composes identity (Affiliation), combat capability (BaseStats),
/// and ability structure (SkillKit).
/// 
/// This entity is NOT responsible for simulation logic.
/// It is the structural definition used by SnapshotResolver.
/// </summary>
public class Hero : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  public Guid HeroClassId { get; private set; }
  public HeroClass HeroClass { get; private set; } = null!;

  // =========================
  // IDENTITY AXIS (COMPOSITE)
  // =========================
  public HeroAffiliation Affiliation { get; private set; } = null!;

  // =========================
  // COMBAT STRUCTURE
  // =========================
  public HeroBaseStats BaseStats { get; private set; } = null!;

  public HeroSkillKit SkillKit { get; private set; } = null!;

  private Hero() { }

  // =========================================================
  // CREATION (ONLY VALID ENTRY POINT)
  // =========================================================
  public void Initialize(
    string name,
    Guid heroClassId,
    HeroAffiliation affiliation,
    HeroBaseStats baseStats,
    string createdBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Hero name is mandatory.");

    if (heroClassId == Guid.Empty)
      throw new ArgumentException("HeroClassId is required.");

    Name = name;
    HeroClassId = heroClassId;

    Affiliation = affiliation ?? throw new ArgumentNullException(nameof(affiliation));
    BaseStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));

    MarkCreated(createdBy);
  }

  // =========================================================
  // IDENTITY EVOLUTION (SAFE MODIFICATION)
  // =========================================================
  public void ChangeAffiliation(HeroAffiliation affiliation)
  {
    if (affiliation is null)
      throw new ArgumentNullException(nameof(affiliation));

    if (Affiliation == affiliation)
      return;

    Affiliation = affiliation;

    MarkUpdated("system");
  }

  public void ChangeClass(Guid heroClassId)
  {
    if (heroClassId == Guid.Empty)
      throw new ArgumentException("Invalid HeroClassId.");

    HeroClassId = heroClassId;

    MarkUpdated("system");
  }

  // =========================================================
  // SKILL SYSTEM
  // =========================================================
  public void SetSkillKit(HeroSkillKit skillKit)
  {
    SkillKit = skillKit ?? throw new ArgumentNullException(nameof(skillKit));

    MarkUpdated("system");
  }
}