using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'écran Profile (Profile tab) : identité joueur, niveau/xp
/// et monnaies. MVP V1 — pas encore d'édition d'avatar, de titres,
/// ni d'historique de matchs (nécessite un Match/History service).
/// </summary>
public class ProfilePanelPresenter : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private PlayerProfileService playerProfileService;

    [Header("Identity")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerIdText;

    [Header("Progression")]
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text xpLabelText;

    [Header("Currencies")]
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text hardCurrencyText;

    private void OnEnable()
    {
        if (playerProfileService == null)
        {
            Debug.LogWarning("[ProfilePanelPresenter] PlayerProfileService is missing.");
            return;
        }

        playerProfileService.OnProfileUpdated += Refresh;
        Refresh(playerProfileService.CurrentProfile);
    }

    private void OnDisable()
    {
        if (playerProfileService != null)
        {
            playerProfileService.OnProfileUpdated -= Refresh;
        }
    }

    private void Refresh(PlayerProfileData profile)
    {
        if (profile == null)
        {
            return;
        }

        if (playerNameText != null)
        {
            playerNameText.text = profile.DisplayName;
        }

        if (playerIdText != null)
        {
            playerIdText.text = profile.HasBackendData ? profile.PlayerId : "Offline profile";
        }

        if (playerLevelText != null)
        {
            playerLevelText.text = $"Level {profile.Level}";
        }

        if (xpBar != null)
        {
            xpBar.value = profile.XpProgress01;
        }

        if (xpLabelText != null)
        {
            xpLabelText.text = $"{profile.Xp} / {profile.XpToNextLevel} XP";
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
}
