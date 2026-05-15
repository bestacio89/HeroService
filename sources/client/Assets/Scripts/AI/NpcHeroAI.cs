using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Heroes;
using UnityEngine;

namespace MobaPrototype.AI
{
    public enum NpcBehaviourMode
    {
        HoldPosition,
        Advance
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(HeroRuntime))]
    [RequireComponent(typeof(HealthSystem))]
    /// <summary>
    /// Gère le comportement de base des héros NPC en combat et en déplacement.
    /// </summary>
    public class NpcHeroAI : MonoBehaviour
    {
        [Header("Behaviour")]
        [SerializeField] private NpcBehaviourMode behaviourMode = NpcBehaviourMode.HoldPosition;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float attackRange = 2.2f;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float attackCooldown = 1.0f;
        [SerializeField] private float attackDamage = 10f;

        [Header("Hold Position Settings")]
        [SerializeField] private float maxChaseDistanceFromAnchor = 4f;
        [SerializeField] private float returnToAnchorDistance = 1f;

        [Header("Advance Settings")]
        [SerializeField] private float advanceStopDistance = 1f;

        [Header("Spacing")]
        [SerializeField] private float separationRadius = 1.5f;
        [SerializeField] private float separationStrength = 1.25f;

        private CharacterController characterController;
        private HeroRuntime heroRuntime;
        private HealthSystem healthSystem;

        private Vector3 anchorPosition;
        private Transform currentTarget;
        private float lastAttackTime = -999f;

        private Vector3 advanceTargetPosition;
        private bool hasAdvanceTarget;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            heroRuntime = GetComponent<HeroRuntime>();
            healthSystem = GetComponent<HealthSystem>();
            anchorPosition = transform.position;
        }

        private void Update()
        {
            if (healthSystem == null || healthSystem.IsDead)
                return;

            currentTarget = FindClosestEnemy();

            switch (behaviourMode)
            {
                case NpcBehaviourMode.HoldPosition:
                    UpdateHoldPosition();
                    break;

                case NpcBehaviourMode.Advance:
                    UpdateAdvance();
                    break;
            }
        }

        public void SetBehaviourMode(NpcBehaviourMode mode)
        {
            behaviourMode = mode;
        }

        public void SetAnchorPosition(Vector3 position)
        {
            anchorPosition = position;
        }

        public void SetAdvanceTargetPosition(Vector3 position)
        {
            advanceTargetPosition = position;
            hasAdvanceTarget = true;
        }

        private void UpdateHoldPosition()
        {
            if (currentTarget == null)
            {
                float distanceToAnchor = Vector3.Distance(transform.position, anchorPosition);
                if (distanceToAnchor > returnToAnchorDistance)
                {
                    MoveTowards(anchorPosition);
                }

                return;
            }

            float distanceToAnchorNow = Vector3.Distance(transform.position, anchorPosition);
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToAnchorNow > maxChaseDistanceFromAnchor)
            {
                ReturnToAnchor();
                return;
            }

            if (distanceToTarget <= attackRange)
            {
                TryAttack(currentTarget);
            }
            else
            {
                MoveTowards(currentTarget.position);
            }
        }

        private void UpdateAdvance()
        {
            if (currentTarget == null)
            {
                if (hasAdvanceTarget)
                {
                    float distanceToAdvanceTarget = Vector3.Distance(transform.position, advanceTargetPosition);
                    if (distanceToAdvanceTarget > advanceStopDistance)
                    {
                        MoveTowards(advanceTargetPosition);
                    }
                }

                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= attackRange)
            {
                TryAttack(currentTarget);
            }
            else
            {
                MoveTowards(currentTarget.position);
            }
        }

        private Transform FindClosestEnemy()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
            float closestDistance = Mathf.Infinity;
            Transform bestTarget = null;

            foreach (Collider hit in hits)
            {
                HealthSystem targetHealth = hit.GetComponentInParent<HealthSystem>();
                if (targetHealth == null || targetHealth == healthSystem || targetHealth.IsDead)
                    continue;

                HeroRuntime targetRuntime = targetHealth.GetComponent<HeroRuntime>();
                if (targetRuntime == null)
                    targetRuntime = targetHealth.GetComponentInParent<HeroRuntime>();

                if (targetRuntime == null)
                    continue;

                if (targetRuntime.Team == heroRuntime.Team)
                    continue;

                float distance = Vector3.Distance(transform.position, targetHealth.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = targetHealth.transform;
                }
            }

            return bestTarget;
        }

        private void MoveTowards(Vector3 targetPosition)
        {
            if (characterController == null || !characterController.enabled)
                return;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            direction.Normalize();

            Vector3 separation = ComputeSeparationVector();
            Vector3 finalDirection = direction + separation * separationStrength;
            finalDirection.y = 0f;

            if (finalDirection.sqrMagnitude <= 0.001f)
                return;

            finalDirection.Normalize();

            characterController.Move(finalDirection * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        private Vector3 ComputeSeparationVector()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);
            Vector3 separation = Vector3.zero;

            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                    continue;

                HeroRuntime otherRuntime = hit.GetComponentInParent<HeroRuntime>();
                if (otherRuntime == null || otherRuntime == heroRuntime)
                    continue;

                if (otherRuntime.Team != heroRuntime.Team)
                    continue;

                Vector3 away = transform.position - otherRuntime.transform.position;
                away.y = 0f;

                float distance = away.magnitude;
                if (distance <= 0.001f)
                    continue;

                float weight = 1f - Mathf.Clamp01(distance / separationRadius);
                separation += away.normalized * weight;
            }

            return separation;
        }

        private void ReturnToAnchor()
        {
            float distance = Vector3.Distance(transform.position, anchorPosition);
            if (distance <= returnToAnchorDistance)
                return;

            MoveTowards(anchorPosition);
        }

        private void TryAttack(Transform target)
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;

            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth == null)
                targetHealth = target.GetComponentInParent<HealthSystem>();

            if (targetHealth == null || targetHealth.IsDead)
                return;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized);
            }

            targetHealth.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, separationRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(
                anchorPosition == Vector3.zero ? transform.position : anchorPosition,
                maxChaseDistanceFromAnchor
            );

            if (hasAdvanceTarget)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(advanceTargetPosition, 0.25f);
            }
        }
    }
}