using MobaPrototype.Systems.Resources;
using MobaPrototype.Systems.Health;
using UnityEngine;

namespace MobaPrototype.Player
{
    /// <summary>
    /// Handles the player's ultimate ability.
    /// Uses the current auto-targeted enemy, applies burst damage,
    /// checks cooldown, checks mana, then spends mana on successful cast.
    /// </summary>
    [RequireComponent(typeof(PlayerTargeting))]
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(ManaSystem))]
    /// <summary>
    /// Déclenche l'ultime du joueur via la compétence équipée.
    /// </summary>
    public sealed class PlayerUltimateAbility : MonoBehaviour
    {
        [Header("Ultimate Settings")]
        [SerializeField] private float ultimateDamage = 40f;
        [SerializeField] private float ultimateRange = 3.5f;
        [SerializeField] private float ultimateCooldown = 8f;
        [SerializeField] private float ultimateManaCost = 80f;

        [Header("Optional")]
        [SerializeField] private bool rotateTowardTargetOnCast = true;
        [SerializeField] private bool drawRangeGizmo = true;

        private PlayerTargeting targeting;
        private HealthSystem selfHealthSystem;
        private ManaSystem manaSystem;

        private float lastCastTime = -999f;

        /// <summary>
        /// Returns true if the ultimate is currently available.
        /// </summary>
        public bool IsReady => Time.time >= lastCastTime + ultimateCooldown;

        /// <summary>
        /// Remaining cooldown in seconds.
        /// </summary>
        public float CooldownRemaining
        {
            get
            {
                float remaining = (lastCastTime + ultimateCooldown) - Time.time;
                return Mathf.Max(0f, remaining);
            }
        }

        /// <summary>
        /// Gets the mana cost of the ultimate.
        /// </summary>
        public float ManaCost => ultimateManaCost;

        private void Awake()
        {
            targeting = GetComponent<PlayerTargeting>();
            selfHealthSystem = GetComponent<HealthSystem>();
            manaSystem = GetComponent<ManaSystem>();
        }

        /// <summary>
        /// Attempts to cast the ultimate on the current target.
        /// Returns true if the cast was successful.
        /// </summary>
        public bool TryCast()
        {
            if (!CanUseCombat())
            {
                return false;
            }

            if (!IsReady)
            {
                return false;
            }

            Transform currentTarget = targeting.CurrentTarget;
            if (!IsTargetValid(currentTarget, out HealthSystem targetHealth))
            {
                return false;
            }

            if (!IsTargetInRange(currentTarget.position))
            {
                return false;
            }

            if (!manaSystem.TrySpendMana(ultimateManaCost))
            {
                return false;
            }

            if (rotateTowardTargetOnCast)
            {
                RotateTowards(currentTarget.position);
            }

            targetHealth.TakeDamage(ultimateDamage);
            lastCastTime = Time.time;

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

            if (!target.TryGetComponent(out targetHealth))
            {
                return false;
            }

            if (targetHealth.IsDead)
            {
                return false;
            }

            return true;
        }

        private bool IsTargetInRange(Vector3 targetPosition)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            return distance <= ultimateRange;
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
            if (!drawRangeGizmo)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ultimateRange);
        }
    }
}