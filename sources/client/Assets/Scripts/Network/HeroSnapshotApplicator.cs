using UnityEngine;
using MobaPrototype.Player;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Resources;

/// <summary>
/// Applique les données d'un HeroSnapshotDto (reçu du backend) sur les composants
/// Unity d'un héros spawné.
///
/// Responsabilités :
/// - injecter les overrides de cooldown et de mana cost sur les HeroSkillSlot,
/// - injecter les HP et Mana initiaux via RestoreFullHealth / RestoreFullMana
///   APRÈS que HeroStatsProvider a été initialisé.
///
/// Contraintes d'architecture :
/// - Ce script est un utilitaire statique pur — aucun MonoBehaviour, aucune dépendance Unity.
/// - HealthSystem.MaxHealth et ManaSystem.MaxMana restent sous contrôle de HeroStatsProvider.
///   On ne les override pas directement — on restaure la santé/mana complète
///   après que HeroDefinition a été assignée par HeroSpawner.
/// - Les cooldowns sont injectés via ApplySnapshotCooldown() défini dans HeroSkillSlot.
///
/// Appelé par :
/// - MatchBootstrapper (Phase 4) après confirmation du héros et réception du snapshot.
/// </summary>
public static class HeroSnapshotApplicator
{
  /// <summary>
  /// Applique le snapshot sur le GameObject héros local.
  /// Doit être appelé APRÈS que HeroSpawner a assigné la HeroDefinition au héros.
  /// </summary>
  public static void Apply(GameObject hero, HeroSnapshotDto snapshot)
  {
    if (hero == null)
    {
      Debug.LogWarning("[HeroSnapshotApplicator] hero est null.");
      return;
    }

    if (snapshot == null)
    {
      Debug.LogWarning("[HeroSnapshotApplicator] snapshot est null.");
      return;
    }

    ApplyHealth(hero);
    ApplyMana(hero);
    ApplySkills(hero, snapshot);
  }

  // ---------------------------------------------------------------
  // HP — restaure la vie complète après initialisation de HeroDefinition
  // ---------------------------------------------------------------

  private static void ApplyHealth(GameObject hero)
  {
    var health = hero.GetComponent<HealthSystem>();

    if (health == null)
    {
      Debug.LogWarning($"[HeroSnapshotApplicator] HealthSystem introuvable sur {hero.name}.");
      return;
    }

    // HeroStatsProvider calcule MaxHealth depuis HeroDefinition.
    // RestoreFullHealth() synchronise currentHealth sur MaxHealth.
    health.RestoreFullHealth();
  }

  // ---------------------------------------------------------------
  // Mana — restaure la mana complète après initialisation de HeroDefinition
  // ---------------------------------------------------------------

  private static void ApplyMana(GameObject hero)
  {
    var mana = hero.GetComponent<ManaSystem>();

    if (mana == null)
    {
      Debug.LogWarning($"[HeroSnapshotApplicator] ManaSystem introuvable sur {hero.name}.");
      return;
    }

    // Même logique que HP — ManaSystem délègue MaxMana à HeroStatsProvider.
    mana.RestoreFullMana();
  }

  // ---------------------------------------------------------------
  // Skills — injecte cooldown et mana cost depuis le snapshot
  // ---------------------------------------------------------------

  private static void ApplySkills(GameObject hero, HeroSnapshotDto snapshot)
  {
    if (snapshot.skills == null) return;

    var kit = snapshot.skills;

    // Primary → SlotSkillOne
    ApplySkillSlot(hero, "SlotSkillOne", kit.primary);

    // Secondary → SlotSkillTwo
    ApplySkillSlot(hero, "SlotSkillTwo", kit.secondary);

    // Ultimate → SlotUltimate
    ApplySkillSlot(hero, "SlotUltimate", kit.ultimate);

    // Passive et Tertiary n'ont pas encore de slot UI dans la scène actuelle.
  }

  private static void ApplySkillSlot(
      GameObject hero,
      string slotName,
      SkillSnapshotDto skillDto)
  {
    if (skillDto == null) return;
    if (skillDto.execution == null) return;

    var child = hero.transform.Find(slotName);

    if (child == null)
    {
      Debug.LogWarning($"[HeroSnapshotApplicator] Enfant '{slotName}' introuvable sur {hero.name}.");
      return;
    }

    var slot = child.GetComponent<HeroSkillSlot>();

    if (slot == null)
    {
      Debug.LogWarning($"[HeroSnapshotApplicator] HeroSkillSlot manquant sur '{slotName}'.");
      return;
    }

    slot.ApplySnapshotCooldown(skillDto.execution.cooldown);
    slot.ApplySnapshotManaCost(skillDto.execution.manaCost);
  }
}