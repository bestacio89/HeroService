// Scripts/Managers/TeamManager.cs
// Gère les 2 équipes et leurs membres.
// Mode-agnostique : ne connaît pas les règles de sélection,
// juste qui appartient à quelle équipe.

using System.Collections.Generic;
using UnityEngine;

namespace MobaPrototype.Managers
{
    /// <summary>
    /// Identifie l'équipe à laquelle appartient une entité runtime.
    /// </summary>
    public enum TeamId { TeamA, TeamB }

    /// <summary>
    /// Enregistre et expose les appartenances d'équipe des héros du runtime.
    /// </summary>
    public class TeamManager : MonoBehaviour
    {
        public static TeamManager Instance { get; private set; }

        private readonly Dictionary<TeamId, List<GameObject>> _teams
            = new Dictionary<TeamId, List<GameObject>>
            {
                { TeamId.TeamA, new List<GameObject>() },
                { TeamId.TeamB, new List<GameObject>() }
            };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Enregistre un héros dans une équipe.
        /// Appelé par HeroSpawner au moment du spawn.
        /// </summary>
        public void RegisterHero(GameObject hero, TeamId team)
        {
            if (!_teams[team].Contains(hero))
                _teams[team].Add(hero);

            ;
        }

        /// <summary>
        /// Retire un héros de son équipe (mort, déconnexion).
        /// </summary>
        public void UnregisterHero(GameObject hero, TeamId team)
        {
            _teams[team].Remove(hero);
        }

        /// <summary>
        /// Retourne tous les membres d'une équipe.
        /// </summary>
        public List<GameObject> GetTeam(TeamId team) => _teams[team];

        /// <summary>
        /// Retourne l'équipe d'un héros donné.
        /// </summary>
        public TeamId GetTeamOf(GameObject hero)
        {
            foreach (var kvp in _teams)
                if (kvp.Value.Contains(hero))
                    return kvp.Key;

            return TeamId.TeamA;
        }

        /// <summary>
        /// Vérifie si deux héros sont dans la même équipe.
        /// </summary>
        public bool AreSameTeam(GameObject a, GameObject b)
            => GetTeamOf(a) == GetTeamOf(b);
    }
}