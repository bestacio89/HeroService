namespace HeroService.Domain.Heroes.Core;
/// <summary>
/// Represents the narrative and mythological lore associated with a Hero.
///
/// Domain Role:
/// Encapsulates purely descriptive and storytelling information about a Hero,
/// including their title, background, and mythological identity.
///
/// This entity exists to support world-building, user experience, and thematic
/// consistency, but it does NOT directly influence gameplay mechanics.
///
/// Matchmaking Relevance:
/// - Has no direct impact on matchmaking, combat simulation, or item scaling.
/// - May be used indirectly for:
///   • UI presentation and Hero selection screens.
///   • Thematic grouping or filtering (non-mechanical).
///   • Player-facing storytelling and immersion layers.
/// - Must NOT be used in rule engines or balance calculations.
///
/// Invariants:
/// - HeroId must reference a valid Hero aggregate.
/// - Title must be non-empty and represent a readable identity descriptor.
/// - Description and BackgroundStory may be empty but should remain coherent if provided.
/// - This entity is strictly 1-to-1 with a Hero.
///
/// Relationships:
/// - Directly associated with a single Hero (HeroId).
/// - Purely presentation-layer dependent:
///   • UI / UX systems
///   • Localization / narrative systems
/// - Explicitly excluded from:
///   • Matchmaking logic
///   • Combat simulation
///   • Item scaling systems
///   • Snapshot evaluation
///
/// Versioning / Snapshot Impact:
/// - No impact on gameplay snapshots.
/// - Changes do NOT require versioning of Hero combat state.
/// - May be versioned independently for storytelling consistency if needed.
///
/// Developer Notes:
/// - This is intentionally separated from gameplay systems to avoid coupling narrative
///   with balance logic.
/// - Do NOT introduce conditional gameplay logic based on Title/Description/Background.
/// - If gameplay relevance is needed, it must be modeled explicitly in a separate domain entity.
/// - Treat this as a presentation-facing aggregate only.
///
/// Architectural Insight:
/// - HeroLore exists to preserve a clean boundary between:
///   • Gameplay simulation (deterministic, balanced systems)
///   • Narrative identity (flexible, expressive content)
/// - This separation is critical to prevent accidental meta coupling through lore.
/// </summary>
public sealed class HeroLore : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  public string Title { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
  public string BackgroundStory { get; private set; } = string.Empty;

  private HeroLore() { }

  public void Define(
      Guid heroId,
      string title,
      string description,
      string background,
      string createdBy)
  {
    if (heroId == Guid.Empty)
      throw new ArgumentException("HeroId is required.");

    if (string.IsNullOrWhiteSpace(title))
      throw new ArgumentException("Title is required.");

    if (string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Description is required.");

    if (string.IsNullOrWhiteSpace(background))
      throw new ArgumentException("Background story is required.");

    HeroId = heroId;
    Title = title;
    Description = description;
    BackgroundStory = background;

    MarkCreated(createdBy);
  }

  public void Revise(
    string title,
    string description,
    string backgroundStory,
    string updatedBy)
  {
    if (string.IsNullOrWhiteSpace(title))
      throw new ArgumentException("Title is required.");

    if (string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Description is required.");

    if (string.IsNullOrWhiteSpace(backgroundStory))
      throw new ArgumentException("Background story is required.");

    Title = title;
    Description = description;
    BackgroundStory = backgroundStory;

    MarkUpdated(updatedBy);
  }
}