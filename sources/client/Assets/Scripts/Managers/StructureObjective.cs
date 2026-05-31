// Scripts/Managers/StructureObjective.cs
using MobaPrototype.Systems.Health;
using UnityEngine;

namespace MobaPrototype.Managers
{
    /// <summary>
    /// Objectif de match RÉEL : termine la partie quand une structure
    /// (Core / Merveille) est détruite.
    ///
    /// Remplace le DebugMatchObjective (qui se déclenchait par un clic manuel).
    /// Hérite de MatchObjectiveSourceBase : il suffit donc de régler
    /// "victoryResult" dans l'inspecteur (l'équipe qui GAGNE quand cette
    /// structure tombe) et d'écouter la mort de la StructureHealth.
    /// </summary>
    [RequireComponent(typeof(StructureHealth))]
    public class StructureObjective : MatchObjectiveSourceBase
    {
        [Header("Source de vie")]
        [Tooltip("La StructureHealth à surveiller. Auto-détectée si laissée vide.")]
        [SerializeField] private StructureHealth structureHealth;

        private void Awake()
        {
            if (structureHealth == null)
            {
                structureHealth = GetComponent<StructureHealth>();
            }
        }

        private void OnEnable()
        {
            if (structureHealth != null)
            {
                structureHealth.OnDied += HandleStructureDestroyed;
            }
        }

        private void OnDisable()
        {
            if (structureHealth != null)
            {
                structureHealth.OnDied -= HandleStructureDestroyed;
            }
        }

        private void HandleStructureDestroyed()
        {
            Debug.Log($"[StructureObjective] Structure '{name}' détruite → fin de match ({VictoryResult}).", this);

            // Méthode héritée de MatchObjectiveSourceBase :
            // vérifie que VictoryResult est valide, trouve le GameManager,
            // puis appelle RequestMatchEnd(VictoryResult, this).
            TriggerObjectiveResolved();
        }
    }
}
