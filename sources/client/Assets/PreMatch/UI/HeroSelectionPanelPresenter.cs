using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'UI de sélection de héros côté joueur.
/// 
/// Responsabilités :
/// - Construire dynamiquement la liste des héros sélectionnables
/// - Gérer la sélection locale d’un héros (UI + état interne)
/// - Synchroniser la sélection avec HeroSelectionSessionService
/// - Permettre la confirmation via un bouton Confirm
/// - Verrouiller la sélection après confirmation
/// - Déclencher la préparation et l’application du MatchLaunchContext
///   via le MatchBootstrapper (passerelle vers le runtime)
/// - Déclencher la transition visuelle vers le runtime
/// - Déclencher l’entrée dans la phase PreGame du start flow runtime
/// 
/// Important :
/// - Ce script ne contient AUCUNE logique gameplay
/// - Il agit uniquement comme orchestrateur UI → Services
/// - Le runtime (HeroSpawner / GameManager / MatchFlow) reste indépendant
/// </summary>
public class HeroSelectionPanelPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform heroListContainer;
    [SerializeField] private HeroSelectionItemView heroSelectionItemPrefab;
    [SerializeField] private Button confirmButton;

    [Header("Services")]
    [SerializeField] private HeroSelectionSessionService sessionService;

    [Header("Bootstrap")]
    [SerializeField] private MatchBootstrapper matchBootstrapper;

    [Header("Transition")]
    [SerializeField] private PreMatchRuntimeTransitionController runtimeTransitionController;

    [Header("Start Flow")]
    [SerializeField] private MatchStartFlowController matchStartFlowController;

    private readonly List<HeroSelectionItemView> spawnedItems = new();
    private string selectedHeroId;

    public string SelectedHeroId => selectedHeroId;

    private void Start()
    {
        BuildHeroList();
        RefreshConfirmButtonState();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        else
        {
            Debug.LogWarning("HeroSelectionPanelPresenter: Confirm Button is missing.");
        }
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirmClicked);
        }
    }

    private void BuildHeroList()
    {
        ClearHeroList();
        selectedHeroId = null;

        // MVP V1 : liste statique
        CreateHeroItem("hero_warrior", "Warrior");
        CreateHeroItem("hero_mage", "Mage");
        CreateHeroItem("hero_archer", "Archer");
        CreateHeroItem("hero_tank", "Tank");
        CreateHeroItem("hero_assassin", "Assassin");
    }

    private void CreateHeroItem(string heroId, string displayName)
    {
        if (heroSelectionItemPrefab == null || heroListContainer == null)
        {
            Debug.LogWarning("HeroSelectionPanelPresenter: Missing references.");
            return;
        }

        HeroSelectionItemView item = Instantiate(heroSelectionItemPrefab, heroListContainer);
        bool isSelected = heroId == selectedHeroId;

        item.Setup(heroId, displayName, isSelected, OnHeroItemClicked);
        spawnedItems.Add(item);
    }

    private void OnHeroItemClicked(string heroId)
    {
        // Lock après confirmation
        if (sessionService != null && sessionService.CurrentSession.IsConfirmed)
        {
            Debug.Log("[HeroSelection] Selection locked: hero already confirmed.");
            return;
        }

        selectedHeroId = heroId;

        if (sessionService != null)
        {
            sessionService.SetHero(heroId);
        }
        else
        {
            Debug.LogWarning("HeroSelectionPanelPresenter: SessionService is missing.");
        }

        RefreshSelectionVisuals();
        RefreshConfirmButtonState();

        Debug.Log($"Hero selected: {heroId}");
    }

    private void OnConfirmClicked()
    {
        if (sessionService == null)
        {
            Debug.LogWarning("HeroSelectionPanelPresenter: SessionService is missing.");
            return;
        }

        bool confirmed = sessionService.Confirm();

        if (!confirmed)
        {
            Debug.LogWarning("[HeroSelection] Confirm failed: no hero selected.");
            RefreshConfirmButtonState();
            return;
        }

        Debug.Log($"[HeroSelection] Selection confirmed: {sessionService.CurrentSession.SelectedHeroId}");

        if (matchBootstrapper == null)
        {
            Debug.LogWarning("[HeroSelection] MatchBootstrapper is missing.");
            RefreshConfirmButtonState();
            return;
        }

        bool runtimeApplied = matchBootstrapper.TryPrepareAndApply();

        if (!runtimeApplied)
        {
            Debug.LogWarning("[HeroSelection] Runtime apply failed.");
            RefreshConfirmButtonState();
            return;
        }

        if (runtimeTransitionController == null)
        {
            Debug.LogWarning("[HeroSelection] PreMatchRuntimeTransitionController is missing.");
            RefreshConfirmButtonState();
            return;
        }

        bool enteredGameplay = runtimeTransitionController.TryEnterGameplay();

        if (!enteredGameplay)
        {
            Debug.LogWarning("[HeroSelection] Runtime transition failed.");
            RefreshConfirmButtonState();
            return;
        }

        if (matchStartFlowController != null)
        {
            bool enteredPreGame = matchStartFlowController.TryEnterPreGame();

            if (!enteredPreGame)
            {
                Debug.LogWarning("[HeroSelection] Failed to enter PreGame.");
                RefreshConfirmButtonState();
                return;
            }

            bool countdownStarted = matchStartFlowController.TryStartCountdown();

            if (!countdownStarted)
            {
                Debug.LogWarning("[HeroSelection] Failed to start countdown.");
                RefreshConfirmButtonState();
                return;
            }
        }
        else
        {
            Debug.LogWarning("[HeroSelection] MatchStartFlowController is missing.");
        }

        RefreshConfirmButtonState();
    }

    private void RefreshSelectionVisuals()
    {
        foreach (HeroSelectionItemView item in spawnedItems)
        {
            if (item == null)
            {
                continue;
            }

            item.SetSelected(item.HeroId == selectedHeroId);
        }
    }

    private void RefreshConfirmButtonState()
    {
        if (confirmButton == null)
        {
            return;
        }

        if (sessionService == null)
        {
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable =
            sessionService.CanConfirm() &&
            !sessionService.CurrentSession.IsConfirmed;
    }

    private void ClearHeroList()
    {
        foreach (HeroSelectionItemView item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        spawnedItems.Clear();
    }
}