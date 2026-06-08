using MobaPrototype.Systems.Heroes;
using MobaPrototype.Systems.Health;
using UnityEngine;

namespace Player
{
  /// <summary>
  /// Handles the player's basic attack.
  /// Supports direct UI button triggering for mobile.
  ///
  /// Étape B4 : l'attaque frappe n'importe quel IDamageable (héros OU Core)
  /// via l'IDamageable résolu par le ciblage (PlayerTargeting.CurrentTargetDamageable).
  ///
  /// DIAGNOSTIC : enableAttackDebugLogs trace chaque tentative d'attaque dans la Console.
  /// À repasser sur false une fois le combat validé.
  /// </summary>
  [RequireComponent(typeof(PlayerTargeting))]
  [RequireComponent(typeof(HealthSystem))]
  public sealed class PlayerBasicAttack : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private HeroStatsProvider heroStatsProvider;
    [SerializeField] private PlayerInputGate inputGate;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 0.8f;

    [Header("PC Test Input")]
    [SerializeField] private bool allowKeyboardInput = false;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;

    [Header("Debug")]
    [Tooltip("Diagnostic B4 : logge chaque tentative d'attaque dans la Console.")]
    [SerializeField] private bool enableAttackDebugLogs = true;

    private PlayerTargeting targeting;
    private HealthSystem selfHealthSystem;
    private float lastAttackTime = -999f;

    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

    public float CurrentAttackDamage
    {
      get
      {
        if (heroStatsProvider != null)
        {
          return Mathf.Max(0f, heroStatsProvider.AttackDamage);
        }

        return Mathf.Max(0f, attackDamage);
      }
    }

    private void Reset()
    {
      heroStatsProvider = GetComponent<HeroStatsProvider>();
      inputGate = GetComponent<PlayerInputGate>();
    }

    private void Awake()
    {
      targeting = GetComponent<PlayerTargeting>();
      selfHealthSystem = GetComponent<HealthSystem>();

      if (heroStatsProvider == null)
      {
        heroStatsProvider = GetComponent<HeroStatsProvider>();
      }

      if (inputGate == null)
      {
        inputGate = GetComponent<PlayerInputGate>();
      }
    }

    private void Update()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!allowKeyboardInput)
            {
                return;
            }

            if (inputGate != null && !inputGate.IsGameplayInputAllowed())
            {
                return;
            }

            if (Input.GetKeyDown(attackKey))
            {
                TryAttack();
            }
#endif
    }

    /// <summary>
    /// Attempts to attack the current target.
    /// Returns true if the attack was successfully executed.
    /// </summary>
    public bool TryAttack()
    {
      if (inputGate != null && !inputGate.IsGameplayInputAllowed())
      {
        Log("Input gameplay bloqué (inputGate) — aucune attaque.");
        return false;
      }

      if (!CanUseCombat())
      {
        Log("Combat indisponible (joueur mort, ou pas de HealthSystem).");
        return false;
      }

      if (!CanAttack)
      {
        // Cooldown en cours : silencieux, sinon la Console serait spammée à chaque frame.
        return false;
      }

      // À partir d'ici, c'est une VRAIE tentative d'attaque (cooldown prêt).
      Transform currentTarget = targeting.CurrentTarget;
      IDamageable targetDamageable = targeting.CurrentTargetDamageable;

      Log($"Tentative d'attaque — cible courante : {(currentTarget == null ? "AUCUNE" : currentTarget.name)}");

      if (!IsTargetValid(currentTarget, targetDamageable))
      {
        Log("  → cible invalide (null ou morte). Aucune attaque.");
        return false;
      }

      float distance = Vector3.Distance(transform.position, currentTarget.position);
      if (distance > attackRange)
      {
        Log($"  → '{currentTarget.name}' HORS DE PORTÉE : {distance:F1} > {attackRange}. Rapproche-toi.");
        return false;
      }

      RotateTowards(currentTarget.position);

      // On frappe via l'interface : fonctionne identiquement sur un héros et sur le Core.
      targetDamageable.TakeDamage(CurrentAttackDamage);
      Log($"  → FRAPPE '{currentTarget.name}' pour {CurrentAttackDamage} dégâts (distance {distance:F1}).");
      lastAttackTime = Time.time;

      return true;
    }

    private bool CanUseCombat()
    {
      if (selfHealthSystem == null)
      {
        return false;
      }

      return !selfHealthSystem.IsDead;
    }

    private bool IsTargetValid(Transform target, IDamageable targetDamageable)
    {
      if (target == null)
      {
        return false;
      }

      if (targetDamageable == null)
      {
        return false;
      }

      if (targetDamageable.IsDead)
      {
        return false;
      }

      return true;
    }

    private void RotateTowards(Vector3 worldPosition)
    {
      Vector3 direction = worldPosition - transform.position;
      direction.y = 0f;

      if (direction.sqrMagnitude <= 0.0001f)
      {
        return;
      }

      Quaternion targetRotation = Quaternion.LookRotation(direction);
      transform.rotation = targetRotation;
    }

    private void Log(string message)
    {
      if (enableAttackDebugLogs)
      {
        Debug.Log($"[PlayerBasicAttack] {message}", this);
      }
    }

    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.red;

      Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
      Gizmos.DrawWireSphere(center, attackRange);
    }
  }
}
