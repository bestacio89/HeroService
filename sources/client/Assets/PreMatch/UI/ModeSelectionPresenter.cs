using UnityEngine;

/// <summary>
/// Gère l'écran de sélection du mode de jeu et la transition UI vers le panneau
/// de sélection de héros.
/// Ce presenter reste limité au pré-match et délègue la persistance du mode
/// au HeroSelectionSessionService.
/// </summary>
public class ModeSelectionPresenter : MonoBehaviour
{
    [Header("Session")]
    [SerializeField] private HeroSelectionSessionService sessionService;

    [Header("Panels")]
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject heroSelectionPanel;

    private void Awake()
    {
        if (heroSelectionPanel != null)
        {
            heroSelectionPanel.SetActive(false);
        }

        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(true);
        }
    }

    public void OnClassicSelected()
    {
        SelectMode(GameModeType.Classic);
    }

    public void OnRankedSelected()
    {
        SelectMode(GameModeType.Ranked);
    }

    public void OnArcadeSelected()
    {
        SelectMode(GameModeType.Arcade);
    }

    private void SelectMode(GameModeType mode)
    {
        if (sessionService == null)
        {
            Debug.LogError("ModeSelectionPresenter: sessionService is not assigned.");
            return;
        }

        sessionService.ResetSession();
        sessionService.SetMode(mode);

        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(false);
        }

        if (heroSelectionPanel != null)
        {
            heroSelectionPanel.SetActive(true);
        }
    }
}
