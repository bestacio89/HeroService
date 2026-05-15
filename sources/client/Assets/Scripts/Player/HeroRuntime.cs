using System;
using UnityEngine;
using MobaPrototype.Managers;

namespace MobaPrototype.Systems.Heroes
{
    /// <summary>
    /// Holds the current hero selection and team on a runtime entity.
    /// This component does not calculate stats by itself.
    /// </summary>
    public class HeroRuntime : MonoBehaviour
    {
        [Header("Hero Setup")]
        [SerializeField] private HeroDefinition heroDefinition;

        [Header("Team")]
        [SerializeField] private TeamId team;

        /// <summary>
        /// Raised when the hero definition changes.
        /// </summary>
        public event Action<HeroDefinition> OnHeroDefinitionChanged;

        /// <summary>
        /// Gets the currently assigned hero definition.
        /// </summary>
        public HeroDefinition HeroDefinition => heroDefinition;

        /// <summary>
        /// Gets the team this hero belongs to.
        /// </summary>
        public TeamId Team => team;

        /// <summary>
        /// Assigns a new hero definition at runtime.
        /// </summary>
        public void SetHeroDefinition(HeroDefinition newHeroDefinition)
        {
            if (heroDefinition == newHeroDefinition) return;
            heroDefinition = newHeroDefinition;
            OnHeroDefinitionChanged?.Invoke(heroDefinition);
        }

        /// <summary>
        /// Appelé par HeroSpawner pour assigner l'équipe au runtime.
        /// </summary>
        public void SetTeam(TeamId newTeam)
        {
            team = newTeam;
        }
    }
}