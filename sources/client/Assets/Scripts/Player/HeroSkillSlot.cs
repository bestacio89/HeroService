using UnityEngine;
using MobaPrototype.Heroes;

namespace MobaPrototype.Player
{
  /// <summary>
  /// Relie une action UI à une compétence de héros.
  ///
  /// Responsabilités :
  /// - déléguer TryCast() au IHeroSkill lié,
  /// - exposer CooldownRemaining, CooldownMax, IsReady pour le HUD,
  /// - accepter des overrides de stats depuis un HeroSnapshot backend
  ///   (CooldownMax, ManaCost) sans modifier la logique du IHeroSkill.
  ///
  /// Contraintes d'architecture :
  /// - Les overrides snapshot sont des valeurs de présentation et d'initialisation.
  /// - CooldownRemaining reste toujours délégué au IHeroSkill (source de vérité runtime).
  /// - ManaCost est stocké ici car IHeroSkill ne l'expose pas encore.
  /// </summary>
  public class HeroSkillSlot : MonoBehaviour
  {
    private IHeroSkill _skill;

    // Overrides reçus depuis le HeroSnapshot backend.
    // Null tant qu'aucun snapshot n'a été appliqué.
    private float? _cooldownMaxOverride;
    private float? _manaCostOverride;

    // ---------------------------------------------------------------
    // Binding
    // ---------------------------------------------------------------

    public void Bind(IHeroSkill skill)
    {
      _skill = skill;
    }

    // ---------------------------------------------------------------
    // Actions
    // ---------------------------------------------------------------

    public void TryCast()
    {
      if (_skill == null)
      {
        Debug.LogWarning($"[HeroSkillSlot] Aucune compétence liée sur {gameObject.name}", this);
        return;
      }

      _skill.TryCast();
    }

    // ---------------------------------------------------------------
    // Propriétés exposées au HUD
    // ---------------------------------------------------------------

    /// <summary>
    /// Temps de cooldown restant — toujours délégué au skill (source de vérité runtime).
    /// </summary>
    public float CooldownRemaining => _skill?.CooldownRemaining ?? 0f;

    /// <summary>
    /// Durée totale du cooldown.
    /// Si un override snapshot a été appliqué, il prend la priorité sur la valeur du skill.
    /// </summary>
    public float CooldownMax => _cooldownMaxOverride ?? _skill?.CooldownMax ?? 1f;

    /// <summary>
    /// Coût en mana de la compétence.
    /// Vient du snapshot backend. Retourne 0 si non encore initialisé.
    /// </summary>
    public float ManaCost => _manaCostOverride ?? 0f;

    public bool IsReady => _skill?.IsReady ?? false;
    public bool IsBound => _skill != null;

    // ---------------------------------------------------------------
    // Overrides snapshot — appelés par HeroSnapshotApplicator (Phase 4)
    // ---------------------------------------------------------------

    /// <summary>
    /// Applique la valeur de cooldown reçue du backend (SkillExecutionSnapshotDto.Cooldown).
    /// N'affecte pas la logique interne du skill — uniquement la valeur affichée dans le HUD.
    /// </summary>
    public void ApplySnapshotCooldown(float cooldownMax)
    {
      if (cooldownMax <= 0f)
      {
        Debug.LogWarning($"[HeroSkillSlot] CooldownMax invalide ({cooldownMax}) sur {gameObject.name}", this);
        return;
      }

      _cooldownMaxOverride = cooldownMax;
    }

    /// <summary>
    /// Applique le coût en mana reçu du backend (SkillExecutionSnapshotDto.ManaCost).
    /// </summary>
    public void ApplySnapshotManaCost(float manaCost)
    {
      if (manaCost < 0f)
      {
        Debug.LogWarning($"[HeroSkillSlot] ManaCost invalide ({manaCost}) sur {gameObject.name}", this);
        return;
      }

      _manaCostOverride = manaCost;
    }

    /// <summary>
    /// Réinitialise les overrides snapshot — à appeler si le héros change de version de jeu.
    /// </summary>
    public void ClearSnapshotOverrides()
    {
      _cooldownMaxOverride = null;
      _manaCostOverride = null;
    }
  }
}
