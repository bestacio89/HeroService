using MobaPrototype.Managers;
using UnityEngine;

/// <summary>
/// Gère la transition UX entre le pré-match et le runtime gameplay.
///
/// Responsabilités :
/// - masquer le pré-match (ModeSelectionPanel + HeroSelectionPanel),
/// - activer le HUD runtime,
/// - connecter la caméra au héros local runtime.
///
/// Important :
/// - aucune logique gameplay,
/// - ne remplace ni GameManager ni HeroSpawner,
/// - ne modifie pas l'état du match,
/// - agit uniquement comme contrôleur de transition UI/runtime.
/// </summary>
public class PreMatchRuntimeTransitionController : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject preMatchRoot;
    [SerializeField] private GameObject heroSelectionPanel;
    [SerializeField] private GameObject gameplayHudRoot;

    [Header("Runtime References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraFollowTopDown cameraFollow;

    [Header("Startup State")]
    [SerializeField] private bool preMatchVisibleOnStart = true;
    [SerializeField] private bool gameplayHudVisibleOnStart = false;

    private bool hasEnteredGameplay;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (preMatchRoot != null)
            preMatchRoot.SetActive(preMatchVisibleOnStart);

        if (heroSelectionPanel != null)
            heroSelectionPanel.SetActive(false);

        if (gameplayHudRoot != null)
            gameplayHudRoot.SetActive(gameplayHudVisibleOnStart);
    }

    public bool TryEnterGameplay()
    {
        if (hasEnteredGameplay)
            return true;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogWarning("[PreMatchRuntimeTransition] GameManager missing.");
            return false;
        }

        if (gameManager.LocalHero == null)
        {
            Debug.LogWarning("[PreMatchRuntimeTransition] LocalHero is not ready.");
            return false;
        }

        if (cameraFollow != null)
            cameraFollow.SetTarget(gameManager.LocalHero.transform);

        if (preMatchRoot != null)
            preMatchRoot.SetActive(false);

        if (heroSelectionPanel != null)
            heroSelectionPanel.SetActive(false);

        if (gameplayHudRoot != null)
            gameplayHudRoot.SetActive(true);

        hasEnteredGameplay = true;

        Debug.Log("[PreMatchRuntimeTransition] Entered gameplay runtime.");
        return true;
    }
}