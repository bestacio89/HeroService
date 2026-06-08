using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Systems.Health;
using MobaPrototype.Player;

namespace MobaPrototype.Heroes.Byakuya
{
  /// <summary>
  /// Exécute l'attaque de base du héros Byakuya.
  ///
  /// Étape B4 (vrai emplacement) : l'attaque frappe désormais n'importe quel
  /// IDamageable — un héros comme un Core — au lieu d'exiger un HealthSystem.
  /// Elle réutilise l'IDamageable résolu par le ciblage (PlayerTargeting, B3).
  /// </summary>
  public class ByakuyaBasicAttack : MonoBehaviour, IHeroSkill
  {
    [Header("Attack Config")]
    [SerializeField] private float baseDamage = 121f;
    [SerializeField] private float petalBonus = 18f;
    [SerializeField] private float range = 3.2f;
    [SerializeField] private float cooldown = 0.97f;

    [Header("References")]
    [SerializeField] private PlayerTargeting playerTargeting;
    [SerializeField] private ByakuyaSkillOne skillOne;

    [Header("VFX (optionnel)")]
    [SerializeField] private ParticleSystem swordStrikeVfx;
    [SerializeField] private ParticleSystem petalStrikeVfx;

    [Header("Debug")]
    [Tooltip("Diagnostic B4 : logge chaque tentative d'attaque dans la Console.")]
    [SerializeField] private bool enableAttackDebugLogs = true;

    private float _cooldownRemaining;

    public float CooldownRemaining => _cooldownRemaining;
    public float CooldownMax => cooldown;
    public bool IsReady => _cooldownRemaining <= 0f;

    private void Awake()
    {
      if (playerTargeting == null)
        playerTargeting = GetComponent<PlayerTargeting>();

      if (skillOne == null)
        skillOne = GetComponent<ByakuyaSkillOne>();

      if (playerTargeting == null)
        Debug.LogError("[ByakuyaBasicAttack] PlayerTargeting introuvable.", this);
    }

    private void Update()
    {
      if (_cooldownRemaining > 0f)
        _cooldownRemaining -= Time.deltaTime;
    }

    public void TryCast()
    {
      if (_cooldownRemaining > 0f)
        return;

      if (playerTargeting == null)
      {
        Debug.LogError("[ByakuyaBasicAttack] PlayerTargeting NULL.", this);
        return;
      }

      Transform target = playerTargeting.CurrentTarget;
      if (target == null)
      {
        Log("Aucune cible courante — rien à frapper.");
        return;
      }

      float distance = Vector3.Distance(transform.position, target.position);
      if (distance > range)
      {
        Log($"'{target.name}' HORS DE PORTÉE : {distance:F1} > {range}. Rapproche-toi.");
        return;
      }

      // B4 : on récupère la cible comme IDamageable (héros OU Core), via le ciblage (B3).
      IDamageable targetDamageable = playerTargeting.CurrentTargetDamageable;
      if (targetDamageable == null || targetDamageable.IsDead)
      {
        Log($"'{target.name}' : cible non attaquable (pas d'IDamageable, ou déjà morte).");
        return;
      }

      _cooldownRemaining = cooldown;

      float damage = baseDamage;
      bool shikaiActive = skillOne != null && !skillOne.IsReady;

      if (shikaiActive)
      {
        damage += petalBonus;
        petalStrikeVfx?.Play();
      }
      else
      {
        swordStrikeVfx?.Play();
      }

      // B4 : on frappe via l'interface — fonctionne sur un héros comme sur le Core.
      targetDamageable.TakeDamage(damage);
      Log($"FRAPPE '{target.name}' pour {damage} dégâts (distance {distance:F1}).");
    }

    private void Log(string message)
    {
      if (enableAttackDebugLogs)
        Debug.Log($"[ByakuyaBasicAttack] {message}", this);
    }

    private void OnDrawGizmosSelected()
    {
      Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.2f);
      Gizmos.DrawWireSphere(transform.position, range);
    }
  }
}
