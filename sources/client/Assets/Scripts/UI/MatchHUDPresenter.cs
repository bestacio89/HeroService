using MobaPrototype.Managers;
using MobaPrototype.Systems.Heroes;
using TMPro;
using UnityEngine;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Met à jour le HUD du match à partir de l'état exposé par le GameManager.
    /// </summary>
    public class MatchHUDPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;

        [Header("UI")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text resultText;

        private void Awake()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;
        }

        private void Start()
        {
            RefreshAll();
        }

        private void OnEnable()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;

            if (gameManager == null)
                return;

            gameManager.OnMatchStarted += HandleMatchStarted;
            gameManager.OnMatchEnded += HandleMatchEnded;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (gameManager == null)
                return;

            gameManager.OnMatchStarted -= HandleMatchStarted;
            gameManager.OnMatchEnded -= HandleMatchEnded;
        }

        private void Update()
        {
            if (gameManager == null)
                return;

            RefreshTimer();
            RefreshState();
        }

        private void HandleMatchStarted()
        {
            RefreshAll();
        }

        private void HandleMatchEnded(MatchResult result)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshTimer();
            RefreshState();
            RefreshResult();
        }

        private void RefreshTimer()
        {
            if (timerText == null || gameManager == null)
                return;

            timerText.text = gameManager.GetFormattedMatchTime();
        }

        private void RefreshState()
        {
            if (stateText == null || gameManager == null)
                return;

            stateText.text = FormatMatchState(gameManager.CurrentState);
        }

        private void RefreshResult()
        {
            if (resultText == null || gameManager == null)
                return;

            bool hasResult = gameManager.CurrentResult != MatchResult.None;
            resultText.gameObject.SetActive(hasResult);

            if (!hasResult)
                return;

            resultText.text = FormatMatchResult(gameManager.CurrentResult);
        }

        private static string FormatMatchState(MatchState state)
        {
            return state switch
            {
                MatchState.Booting => "Booting",
                MatchState.Playing => "Playing",
                MatchState.Paused => "Paused",
                MatchState.Ended => "Ended",
                _ => state.ToString()
            };
        }

        private string FormatMatchResult(MatchResult result)
        {
            GameObject localHeroObject = gameManager.LocalHero;

            if (result == MatchResult.Draw)
                return "Draw";

            if (localHeroObject == null)
            {
                return result switch
                {
                    MatchResult.TeamAWin => "Team A Victory",
                    MatchResult.TeamBWin => "Team B Victory",
                    _ => string.Empty
                };
            }

            HeroRuntime localHeroRuntime = localHeroObject.GetComponent<HeroRuntime>();

            if (localHeroRuntime == null)
            {
                return result switch
                {
                    MatchResult.TeamAWin => "Team A Victory",
                    MatchResult.TeamBWin => "Team B Victory",
                    _ => string.Empty
                };
            }

            bool localIsTeamA = localHeroRuntime.Team == TeamId.TeamA;

            return result switch
            {
                MatchResult.TeamAWin => localIsTeamA ? "Victory" : "Defeat",
                MatchResult.TeamBWin => localIsTeamA ? "Defeat" : "Victory",
                MatchResult.Draw => "Draw",
                _ => string.Empty
            };
        }
    }
}