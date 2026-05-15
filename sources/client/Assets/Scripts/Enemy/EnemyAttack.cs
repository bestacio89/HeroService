using UnityEngine;
using MobaPrototype.Systems.Health;

namespace Enemy
{
    /// <summary>
    /// Handles enemy attack logic and damage application on the player.
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.2f;

        private float lastAttackTime = -999f;

        /// <summary>
        /// Returns true if the given target is inside attack range.
        /// </summary>
        public bool IsInRange(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= attackRange;
        }

        /// <summary>
        /// Attempts to attack the target.
        /// Returns true if the attack was successfully executed.
        /// </summary>
        public bool TryAttack(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            if (Time.time < lastAttackTime + attackCooldown)
            {
                return false;
            }

            HealthSystem targetHealth = target.GetComponent<HealthSystem>();

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

            RotateTowards(target.position);

            targetHealth.TakeDamage(attackDamage);
            lastAttackTime = Time.time;

            return true;
        }

        private void RotateTowards(Vector3 targetPosition)
        {
            Vector3 lookDirection = targetPosition - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}