using System.Collections.Generic;
using UnityEngine;
using MobaPrototype.Systems.Health;
using MobaPrototype.Managers;
using MobaPrototype.Systems.Heroes;

/// <summary>
/// Ciblage du joueur (B3) + DIAGNOSTIC B4 : liste chaque cible détectée,
/// son équipe et la raison pour laquelle elle est retenue ou rejetée.
/// Mets enableDebugLogs à false une fois le souci réglé.
/// </summary>
public class PlayerTargeting : MonoBehaviour
{
  [SerializeField] private float targetingRange = 12f;
  [SerializeField] private LayerMask targetLayer;
  [SerializeField] private bool enableDebugLogs = true;

  public Transform CurrentTarget { get; private set; }
  public IDamageable CurrentTargetDamageable { get; private set; }

  private HeroRuntime selfHeroRuntime;
  private Transform lastLoggedTarget;
  private float nextDiagTime;

  private void Awake()
  {
    selfHeroRuntime = GetComponent<HeroRuntime>();
  }

  private void Update()
  {
    FindClosestTarget();

    if (lastLoggedTarget != CurrentTarget)
    {
      if (enableDebugLogs)
        Debug.Log($"[PlayerTargeting] Cible verrouillée → {(CurrentTarget == null ? "AUCUNE" : CurrentTarget.name)}", this);

      lastLoggedTarget = CurrentTarget;
    }
  }

  private void FindClosestTarget()
  {
    Collider[] hits = Physics.OverlapSphere(transform.position, targetingRange, targetLayer);

    // Diagnostic limité à 1 fois par seconde pour ne pas saturer la Console.
    bool diag = enableDebugLogs && Time.time >= nextDiagTime;
    if (diag)
    {
      nextDiagTime = Time.time + 1f;
      Debug.Log($"[PlayerTargeting] === {hits.Length} colliders dans le rayon | mon équipe = {(selfHeroRuntime == null ? "?" : selfHeroRuntime.Team.ToString())} ===", this);
    }

    if (selfHeroRuntime == null)
    {
      CurrentTarget = null;
      CurrentTargetDamageable = null;
      return;
    }

    HashSet<IDamageable> uniqueTargets = new();
    float closestDistance = Mathf.Infinity;
    Transform bestTarget = null;
    IDamageable bestDamageable = null;

    foreach (Collider hit in hits)
    {
      IDamageable targetDamageable = hit.GetComponentInParent<IDamageable>();
      if (targetDamageable == null)
        continue;

      // On ne traite (et ne logge) chaque objet qu'une fois.
      if (!uniqueTargets.Add(targetDamageable))
        continue;

      bool isSelf = targetDamageable.Transform == transform;
      bool isDead = targetDamageable.IsDead;
      bool isAlly = targetDamageable.Team == selfHeroRuntime.Team;
      float distance = Vector3.Distance(transform.position, targetDamageable.Transform.position);

      if (diag)
      {
        string verdict =
            isSelf ? "REJET : c'est moi"
            : isDead ? "REJET : déjà mort"
            : isAlly ? $"REJET : allié (équipe {targetDamageable.Team})"
            : $"CANDIDAT VALIDE (équipe {targetDamageable.Team}, distance {distance:F1})";

        Debug.Log($"[PlayerTargeting]   • {targetDamageable.Transform.name} → {verdict}", this);
      }

      if (isSelf || isDead || isAlly)
        continue;

      if (distance < closestDistance)
      {
        closestDistance = distance;
        bestTarget = targetDamageable.Transform;
        bestDamageable = targetDamageable;
      }
    }

    CurrentTarget = bestTarget;
    CurrentTargetDamageable = bestDamageable;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, targetingRange);
  }
}