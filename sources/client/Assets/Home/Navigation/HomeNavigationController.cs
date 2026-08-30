using System;
using UnityEngine;

/// <summary>
/// Gère la navigation principale du Home/Lobby : bascule entre les panneaux
/// Home, Heroes, Shop et Profile via la barre de navigation inférieure.
///
/// Même philosophie que ModeSelectionPresenter côté pré-match :
/// pas de framework de navigation, juste un SetActive contrôlé
/// sur un petit nombre de panneaux connus à l'avance.
///
/// Ce controller ne contient AUCUNE logique métier : il délègue tout
/// le contenu de chaque panneau à son propre presenter (HomeLandingPresenter,
/// HeroCollectionPresenter, ShopPanelPresenter, ProfilePanelPresenter).
/// </summary>
public class HomeNavigationController : MonoBehaviour
{
    [Serializable]
    private struct TabEntry
    {
        public HomeTabType tab;
        public GameObject panel;
        public NavTabButtonView navButton;
    }

    [Header("Tabs")]
    [SerializeField] private TabEntry[] tabs = Array.Empty<TabEntry>();

    [Header("Startup")]
    [SerializeField] private HomeTabType startingTab = HomeTabType.Home;

    public event Action<HomeTabType> OnTabChanged;

    public HomeTabType CurrentTab { get; private set; }

    private void Awake()
    {
        foreach (TabEntry entry in tabs)
        {
            if (entry.navButton != null)
            {
                entry.navButton.Setup(SelectTab);
            }
        }
    }

    private void Start()
    {
        SelectTab(startingTab);
    }

    public void SelectTab(HomeTabType tab)
    {
        CurrentTab = tab;

        foreach (TabEntry entry in tabs)
        {
            bool isActive = entry.tab == tab;

            if (entry.panel != null)
            {
                entry.panel.SetActive(isActive);
            }

            if (entry.navButton != null)
            {
                entry.navButton.SetSelected(isActive);
            }
        }

        OnTabChanged?.Invoke(tab);
    }
}
