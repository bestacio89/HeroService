using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'écran d'accueil (Home tab) : barre de statut joueur (nom, niveau,
/// monnaies) et bouton Battle qui déclenche la transition vers le match.
///
/// Ce presenter reste limité à l'affichage et à l'orchestration UI → services :
/// - lit PlayerProfileService pour la barre de statut,
/// - délègue le lancement de partie à MainMenuSceneLoader.
/// </summary>
public class HomeLandingPresenter : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private PlayerProfileService playerProfileService;
    [SerializeField] private MainMenuSceneLoader sceneLoader;

    [Header("Status Bar")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text hardCurrencyText;

    [Header("Battle CTA")]
    [SerializeField] private Button battleButton;

    private void Awake()
    {
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(OnBattleClicked);
        }
    }

    private void OnEnable()
    {
        if (playerProfileService == null)
        {
            Debug.LogWarning("[HomeLandingPresenter] PlayerProfileService is missing.");
            return;
        }

        playerProfileService.OnProfileUpdated += RefreshStatusBar;
        RefreshStatusBar(playerProfileService.CurrentProfile);
    }

    private void OnDisable()
    {
        if (playerProfileService != null)
        {
            playerProfileService.OnProfileUpdated -= RefreshStatusBar;
        }
    }

    private void OnDestroy()
    {
        if (battleButton != null)
        {
            battleButton.onClick.RemoveListener(OnBattleClicked);
        }
    }

    private void RefreshStatusBar(PlayerProfileData profile)
    {
        if (profile == null)
        {
            return;
        }

        if (playerNameText != null)
        {
            playerNameText.text = profile.DisplayName;
        }

        if (playerLevelText != null)
        {
            playerLevelText.text = $"Lv. {profile.Level}";
        }

        if (xpBar != null)
        {
            xpBar.value = profile.XpProgress01;
        }

        if (softCurrencyText != null)
        {
            softCurrencyText.text = profile.SoftCurrency.ToString();
        }

        if (hardCurrencyText != null)
        {
            hardCurrencyText.text = profile.HardCurrency.ToString();
        }
    }

    private void OnBattleClicked()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("[HomeLandingPresenter] MainMenuSceneLoader is missing.");
            return;
        }

        if (battleButton != null)
        {
            battleButton.interactable = false;
        }

        sceneLoader.StartMatch();
    }
}
