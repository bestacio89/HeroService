using UnityEngine;

namespace MobaPrototype.Managers
{
    /// <summary>
    /// Stub concret d'objectif de match.
    /// Sert uniquement à valider la chaîne complète :
    /// objectif externe -> GameManager.RequestMatchEnd() -> EndMatch()
    ///
    /// Ce composant n'est PAS la future Merveille.
    /// Il sera remplacé plus tard par un vrai objectif métier.
    /// </summary>
    public class DebugMatchObjective : MatchObjectiveSourceBase
    {
        [Header("Debug")]
        [SerializeField] private bool logResolution = true;

        [ContextMenu("Trigger Objective Resolved")]
        public void DebugResolveObjective()
        {
            if (logResolution)
            {
                Debug.Log("[DebugMatchObjective] Objective resolved.");
            }

            TriggerObjectiveResolved();
        }
    }
}