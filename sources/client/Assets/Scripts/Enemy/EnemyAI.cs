using UnityEngine;
using MobaPrototype.Systems.Health;

namespace Enemy
{
    /// <summary>
    /// Main enemy brain.
    /// Detects the player, chases the player, and attacks when in range.
    /// </summary>
    [RequireComponent(typeof(EnemyMovement))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(HealthSystem))]
    /// <summary>
    /// Pilote le comportement basique des anciens ennemis de scène.
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float detectionRange = 8f;
        [SerializeField] private Transform playerTarget;

        private EnemyMovement enemyMovement;
        private EnemyAttack enemyAttack;
        private HealthSystem healthSystem;
        private HealthSystem playerHealthSystem;

        private void Awake()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAttack = GetComponent<EnemyAttack>();
            healthSystem = GetComponent<HealthSystem>();
        }

        private void Start()
        {
            if (playerTarget == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    playerTarget = playerObject.transform;
                }
            }

            CachePlayerHealthSystem();
        }

        private void Update()
        {
            if (healthSystem == null || healthSystem.IsDead)
            {
                return;
            }

            if (playerTarget == null)
            {
                enemyMovement.Stop();
                return;
            }

            if (playerHealthSystem == null)
            {
                CachePlayerHealthSystem();

                if (playerHealthSystem == null)
                {
                    enemyMovement.Stop();
                    return;
                }
            }

            if (playerHealthSystem.IsDead)
            {
                enemyMovement.Stop();
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer > detectionRange)
            {
                enemyMovement.Stop();
                return;
            }

            if (enemyAttack.IsInRange(playerTarget))
            {
                enemyMovement.Stop();
                enemyAttack.TryAttack(playerTarget);
            }
            else
            {
                enemyMovement.MoveTowards(playerTarget.position);
            }
        }

        private void CachePlayerHealthSystem()
        {
            if (playerTarget == null)
            {
                playerHealthSystem = null;
                return;
            }

            playerHealthSystem = playerTarget.GetComponent<HealthSystem>();

            if (playerHealthSystem == null)
            {
                playerHealthSystem = playerTarget.GetComponentInParent<HealthSystem>();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}