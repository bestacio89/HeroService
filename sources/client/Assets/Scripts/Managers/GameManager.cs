using System;
using System.Collections.Generic;
using MobaPrototype.Systems.Heroes;
using UnityEngine;

namespace MobaPrototype.Managers
{
    /// <summary>
    /// Représente les différents états possibles du match.
    /// </summary>
    public enum MatchState
    {
        Booting,
        Playing,
        Paused,
        Ended
    }

    /// <summary>
    /// Représente les résultats possibles en fin de match.
    /// </summary>
    public enum MatchResult
    {
        None,
        TeamAWin,
        TeamBWin,
        Draw
    }

    /// <summary>
    /// Centralise l'état global du match, son timer et ses transitions principales.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Runtime State")]
        [SerializeField] private MatchState currentState = MatchState.Booting;
        [SerializeField] private MatchResult currentResult = MatchResult.None;
        [SerializeField] private float matchTime = 0f;

        private readonly List<GameObject> allHeroes = new();
        private readonly List<GameObject> teamAHeroes = new();
        private readonly List<GameObject> teamBHeroes = new();

        private GameObject localHero;

        public event Action OnMatchStarted;
        public event Action<MatchResult> OnMatchEnded;

        public MatchState CurrentState => currentState;
        public MatchResult CurrentResult => currentResult;
        public float MatchTime => matchTime;
        public GameObject LocalHero => localHero;
        public IReadOnlyList<GameObject> AllHeroes => allHeroes;
        public IReadOnlyList<GameObject> TeamAHeroes => teamAHeroes;
        public IReadOnlyList<GameObject> TeamBHeroes => teamBHeroes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            currentState = MatchState.Booting;
            currentResult = MatchResult.None;
            matchTime = 0f;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (currentState == MatchState.Playing)
            {
                matchTime += Time.unscaledDeltaTime;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Time.timeScale = 1f;
                Instance = null;
            }
        }

        public void InitializeMatch(List<GameObject> spawnedHeroes, GameObject spawnedLocalHero)
        {
            allHeroes.Clear();
            teamAHeroes.Clear();
            teamBHeroes.Clear();
            localHero = null;
            matchTime = 0f;
            currentState = MatchState.Booting;
            currentResult = MatchResult.None;
            Time.timeScale = 1f;

            if (spawnedHeroes == null || spawnedHeroes.Count == 0)
            {
                Debug.LogWarning("[GameManager] InitializeMatch appelé sans héros valides.", this);
                return;
            }

            foreach (GameObject hero in spawnedHeroes)
            {
                if (hero == null)
                {
                    continue;
                }

                allHeroes.Add(hero);

                HeroRuntime runtime = hero.GetComponent<HeroRuntime>();
                if (runtime == null)
                {
                    Debug.LogWarning($"[GameManager] HeroRuntime manquant sur {hero.name}.", hero);
                    continue;
                }

                if (runtime.Team == TeamId.TeamA)
                {
                    teamAHeroes.Add(hero);
                }
                else
                {
                    teamBHeroes.Add(hero);
                }
            }

            if (spawnedLocalHero != null)
            {
                localHero = spawnedLocalHero;
            }

            Debug.Log("[GameManager] Match initialized. Waiting for start flow.");
        }

        public void StartMatch()
        {
            if (allHeroes.Count == 0)
            {
                Debug.LogWarning("[GameManager] Impossible de démarrer le match : aucun héros enregistré.", this);
                return;
            }

            if (currentState == MatchState.Ended)
            {
                Debug.LogWarning("[GameManager] Impossible de démarrer : match déjà terminé.", this);
                return;
            }

            Time.timeScale = 1f;
            currentState = MatchState.Playing;
            currentResult = MatchResult.None;

            Debug.Log("[GameManager] Match started.");
            OnMatchStarted?.Invoke();
        }

        public void PauseMatch()
        {
            if (currentState != MatchState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            currentState = MatchState.Paused;
        }

        public void ResumeMatch()
        {
            if (currentState != MatchState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            currentState = MatchState.Playing;
        }

        public void EndMatch(MatchResult result)
        {
            if (currentState == MatchState.Ended)
            {
                return;
            }

            currentState = MatchState.Ended;
            currentResult = result;
            Time.timeScale = 0f;

            OnMatchEnded?.Invoke(result);
        }

        public void RequestMatchEnd(MatchResult result, UnityEngine.Object source = null)
        {
            if (currentState == MatchState.Ended)
            {
                return;
            }

            if (result == MatchResult.None)
            {
                Debug.LogWarning("[GameManager] RequestMatchEnd refusé : résultat invalide.", this);
                return;
            }

            EndMatch(result);
        }

        public bool IsMatchReady()
        {
            return localHero != null && allHeroes.Count > 0;
        }

        public List<GameObject> GetHeroesByTeam(TeamId team)
        {
            return team == TeamId.TeamA
                ? new List<GameObject>(teamAHeroes)
                : new List<GameObject>(teamBHeroes);
        }

        public string GetFormattedMatchTime()
        {
            int totalSeconds = Mathf.FloorToInt(matchTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}