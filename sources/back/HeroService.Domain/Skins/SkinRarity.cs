namespace HeroService.Domain.Skins;

/// <summary>
/// Defines the rarity tier of a Skin.
///
/// Domain Role:
/// SkinRarity represents the **cosmetic acquisition and prestige tier**
/// of a Skin within the Hero system.
///
/// It has NO impact on gameplay, combat simulation, or matchmaking.
///
/// It exists purely to express:
/// - acquisition difficulty
/// - visual prestige hierarchy
/// - collection rarity classification
///
/// -------------------------
/// GAMEPLAY IMPACT
/// -------------------------
/// SkinRarity MUST NOT influence:
/// - Hero stats
/// - Skill behavior
/// - SnapshotResolver outputs
/// - Matchmaking evaluation
///
/// It is strictly a **meta / cosmetic classification system**.
///
/// -------------------------
/// ECONOMIC / SYSTEM USES
/// -------------------------
/// SkinRarity may be used by external systems such as:
/// - shop pricing logic
/// - loot tables
/// - battle pass reward tiers
/// - cosmetic drop probability systems
///
/// These systems are OUTSIDE combat simulation scope.
///
/// -------------------------
/// RARITY SEMANTICS
/// -------------------------
/// Common:
/// - baseline skins
/// - no special effects
///
/// Uncommon:
/// - minor visual variation
///
/// Rare:
/// - noticeable thematic changes
///
/// Epic:
/// - strong visual redesign + VFX upgrades
///
/// Legendary:
/// - full thematic overhaul (model + VFX + SFX)
///
/// Mythic:
/// - highest prestige tier
/// - often limited, event-based, or exclusive
///
/// -------------------------
/// INVARIANTS
/// -------------------------
/// - Rarity is static once assigned to a Skin.
/// - Rarity does not evolve based on gameplay.
/// - Rarity is independent of Hero strength or performance.
/// - Rarity must NOT leak into simulation systems.
///
/// -------------------------
/// ARCHITECTURAL INSIGHT
/// -------------------------
/// SkinRarity belongs strictly to the **cosmetic progression domain**.
///
/// It is orthogonal to:
/// - Hero progression
/// - Skill systems
/// - Game balance
/// - Snapshot system
///
/// Correct separation:
///
/// Gameplay Domain:
/// - HeroBaseStats
/// - SkillBaseStats
/// - Modifiers
/// - SnapshotResolver
///
/// Cosmetic Domain:
/// - Skin
/// - SkinRarity
/// - VFX/SFX bundles
/// </summary>
public enum SkinRarity
{
  Common = 0,
  Uncommon = 1,
  Rare = 2,
  Epic = 3,
  Legendary = 4,
  Mythic = 5
}