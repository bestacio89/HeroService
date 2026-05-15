using UnityEngine;

namespace MobaPrototype.Managers
{
    /// <summary>
    /// Base abstraite pour tout objectif de match futur
    /// (ex: Merveille, Core, objectif principal).
    /// Cette classe ne contient pas encore la logique métier
    /// de destruction ou de capture : elle ne fait que fournir
    /// un point d'entrée propre vers le GameManager.
    /// </summary>
    public abstract class MatchObjectiveSourceBase : MonoBehaviour, IMatchObjectiveSource
    {
        [Header("Objective Result")]
        [SerializeField] private MatchResult victoryResult = MatchResult.None;

        public MatchResult VictoryResult => victoryResult;

        public virtual void TriggerObjectiveResolved()
        {
            if (victoryResult == MatchResult.None)
            {
                Debug.LogWarning($"[MatchObjectiveSourceBase] VictoryResult invalide sur {name}.", this);
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"[MatchObjectiveSourceBase] GameManager.Instance introuvable pour {name}.", this);
                return;
            }

            GameManager.Instance.RequestMatchEnd(victoryResult, this);
        }
    }
}
