using UnityEngine;

/// <summary>
/// Sert de source de vérité de la session de sélection pré-match.
/// Il centralise les choix du joueur local (mode, héros, lane), gère la confirmation
/// et construit le MatchLaunchContext consommé par le bootstrap du runtime.
/// </summary>
public class HeroSelectionSessionService : MonoBehaviour
{
    private HeroSelectionSessionData _currentSession = new HeroSelectionSessionData();

    public HeroSelectionSessionData CurrentSession => _currentSession;

    public void ResetSession()
    {
        _currentSession = new HeroSelectionSessionData();
    }

    public void SetMode(GameModeType mode)
    {
        _currentSession.SelectedMode = mode;
        _currentSession.IsConfirmed = false;
    }

    public void SetHero(string heroId)
    {
        _currentSession.SelectedHeroId = heroId ?? string.Empty;
        _currentSession.IsConfirmed = false;

        Debug.Log($"[HeroSelectionSession] SelectedHeroId = {_currentSession.SelectedHeroId}");
    }

    public void SetLane(LanePreferenceType lane)
    {
        _currentSession.PreferredLane = lane;
        _currentSession.IsConfirmed = false;
    }

    public bool CanConfirm()
    {
        return !string.IsNullOrWhiteSpace(_currentSession.SelectedHeroId);
    }

    public bool Confirm()
    {
        if (!CanConfirm())
        {
            return false;
        }

        _currentSession.IsConfirmed = true;
        return true;
    }

    public MatchLaunchContext BuildMatchLaunchContext()
    {
        return new MatchLaunchContext
        {
            SelectedMode = _currentSession.SelectedMode,
            SelectedHeroId = _currentSession.SelectedHeroId,
            PreferredLane = _currentSession.PreferredLane
        };
    }
}
