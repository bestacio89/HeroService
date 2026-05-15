using MobaPrototype.Systems.Heroes;
using MobaPrototype.Systems.Health;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles the player's basic attack.
    /// Supports direct UI button triggering for mobile.
    /// Optional keyboard input can be enabled only when legacy input is allowed by project settings.
    /// Attack damage can optionally come from HeroStatsProvider.
    /// If no HeroStatsProvider is found, the local fallback attackDamage value is used.
    /// </summary>
    [RequireComponent(typeof(PlayerTargeting))]
    [RequireComponent(typeof(HealthSystem))]
    /// <summary>
    /// Gère l'attaque de base contrôlée par le joueur.
    /// </summary>
    public sealed class PlayerBasicAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private HeroStatsProvider heroStatsProvider;
        [SerializeField] private PlayerInputGate inputGate;

        [Header("Attack Settings")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 5f; // Voir s'il n'est pas possible de mettre 2f
        [SerializeField] private float attackCooldown = 0.8f;

        [Header("PC Test Input")]
        [SerializeField] private bool allowKeyboardInput = false;
        [SerializeField] private KeyCode attackKey = KeyCode.Space;

        private PlayerTargeting targeting;
        private HealthSystem selfHealthSystem;
        private float lastAttackTime = -999f;

        /// <summary>
        /// Returns true when the basic attack can currently be used.
        /// </summary>
        public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

        /// <summary>
        /// Gets the effective attack damage currently used by the player.
        /// Uses hero stats when available, otherwise falls back to the local attackDamage value.
        /// </summary>
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
                return false;
            }

            if (!CanUseCombat())
            {
                return false;
            }

            if (!CanAttack)
            {
                return false;
            }

            Transform currentTarget = targeting.CurrentTarget;
            if (!IsTargetValid(currentTarget, out HealthSystem targetHealth))
            {
                return false;
            }

            float distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance > attackRange)
            {
                return false;
            }

            RotateTowards(currentTarget.position);

            targetHealth.TakeDamage(CurrentAttackDamage);
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

        private bool IsTargetValid(Transform target, out HealthSystem targetHealth)
        {
            targetHealth = null;

            if (target == null)
            {
                return false;
            }

            targetHealth = target.GetComponent<HealthSystem>();

            if (targetHealth == null)
            {
                targetHealth = target.GetComponentInParent<HealthSystem>();
            }

            if (targetHealth == null)
            {
                return false;
            }

            if (targetHealth.IsDead)
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.DrawWireSphere(center, attackRange);
        }
    }
}