namespace HeroService.Domain.Skins;

/// <summary>
/// Represents a cosmetic Skin applied to a Hero.
/// – Domain Role:
/// Skin defines the **visual and audio presentation layer** of a Hero.
/// It does NOT influence gameplay mechanics, combat outcomes, or simulation results.
///
/// Skins exist purely in the **presentation domain layer** and are consumed by:
/// - client rendering systems
/// - animation pipelines
/// - VFX/SFX runtime loaders
///
/// -------------------------
/// GAMEPLAY IMPACT
/// -------------------------
/// Skins MUST NOT:
/// - modify stats
/// - affect Skill behavior
/// - influence matchmaking
/// - alter SnapshotResolver outputs
///
/// They are strictly **non-functional cosmetic assets**.
///
/// -------------------------
/// MATCHMAKING RELEVANCE
/// -------------------------
/// Skins have ZERO influence on matchmaking or combat simulation.
///
/// However, indirectly they may be used for:
/// - player identification
/// - cosmetic rarity signaling (UI only)
/// - monetization segmentation
///
/// -------------------------
/// INVARIANTS
/// -------------------------
/// - Each Skin must be linked to exactly one HeroId.
/// - Name must be unique per Hero context (recommended, not strictly enforced here).
/// - Rarity defines acquisition difficulty and cosmetic tier.
/// - Visual/audio bundles are optional and resolved at client runtime.
/// - Skin must NOT affect any deterministic snapshot system.
///
/// -------------------------
/// RELATIONSHIPS
/// -------------------------
/// Skin is associated with:
/// - Hero (owner entity)
///
/// Skin is consumed by:
/// - Rendering engine
/// - VFX/SFX system
/// - UI customization systems
///
/// Skin is NOT consumed by:
/// - SnapshotResolver
/// - Combat simulation
/// - Matchmaking system
///
/// -------------------------
/// VERSIONING / SNAPSHOT IMPACT
/// -------------------------
/// Skins are completely OUTSIDE the versioned gameplay simulation system.
///
/// They are:
/// - not included in GameVersion snapshots
/// - not affected by balance patches
/// - not part of deterministic simulation outputs
///
/// -------------------------
/// ARCHITECTURAL INSIGHT
/// -------------------------
/// Skins belong to the **presentation domain**, not the **simulation domain**.
///
/// Correct layering:
///
/// Gameplay Layer:
/// - HeroBaseStats
/// - SkillBaseStats
/// - SkillEffects
/// - Modifiers
/// - SnapshotResolver
///
/// Presentation Layer:
/// - Skin
/// - VFX bundles
/// - SFX bundles
/// - UI themes
///
/// These layers must NEVER overlap.
/// </summary>
public class Skin : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  public string Name { get; private set; } = string.Empty;

  public SkinRarity Rarity { get; private set; }

  public string? VisualTheme { get; private set; }

  public string? VfxBundleKey { get; private set; }

  public string? SfxBundleKey { get; private set; }

  private Skin() { }

  public Skin(Guid heroId, string name, SkinRarity rarity)
  {
    HeroId = heroId;
    Name = name;
    Rarity = rarity;

    MarkCreated("system");
  }
}