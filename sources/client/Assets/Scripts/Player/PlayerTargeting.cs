using System.Collections.Generic;
using UnityEngine;
using MobaPrototype.Systems.Health;
using MobaPrototype.Managers;
using MobaPrototype.Systems.Heroes;

/// <summary>
/// Handles the player's target selection system.
/// Automatically selects the closest valid enemy target in range.
/// </summary>
public class PlayerTargeting : MonoBehaviour
{
    [SerializeField] private float targetingRange = 12f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private bool enableDebugLogs = false;

    public Transform CurrentTarget { get; private set; }

    private HealthSystem selfHealthSystem;
    private HeroRuntime selfHeroRuntime;
    private Transform lastLoggedTarget;

    private void Awake()
    {
        selfHealthSystem = GetComponent<HealthSystem>();
        selfHeroRuntime = GetComponent<HeroRuntime>();
    }

    private void Update()
    {
        FindClosestTarget();

        if (lastLoggedTarget != CurrentTarget)
        {
            if (enableDebugLogs)
            {
                ;
            }

            lastLoggedTarget = CurrentTarget;
        }
    }

    private void FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetingRange, targetLayer);

        if (selfHeroRuntime == null)
        {
            if (enableDebugLogs)
                Debug.LogError("[PlayerTargeting] Self HeroRuntime NULL.");

            CurrentTarget = null;
            return;
        }

        HashSet<HealthSystem> uniqueDetectedHeroes = new();

        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;
        int uniqueEnemyCandidates = 0;

        foreach (Collider hit in hits)
        {
            HealthSystem targetHealth = hit.GetComponentInParent<HealthSystem>();
            if (targetHealth == null)
                continue;

            if (targetHealth == selfHealthSystem)
                continue;

            if (targetHealth.IsDead)
                continue;

            if (!uniqueDetectedHeroes.Add(targetHealth))
                continue;

            HeroRuntime targetHeroRuntime = targetHealth.GetComponent<HeroRuntime>();
            if (targetHeroRuntime == null)
                targetHeroRuntime = targetHealth.GetComponentInParent<HeroRuntime>();

            if (targetHeroRuntime == null)
                continue;

            if (targetHeroRuntime.Team == selfHeroRuntime.Team)
                continue;

            uniqueEnemyCandidates++;

            float distance = Vector3.Distance(transform.position, targetHealth.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = targetHealth.transform;
            }
        }

        CurrentTarget = bestTarget;

        if (enableDebugLogs)
        {
            ;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetingRange);
    }
}
