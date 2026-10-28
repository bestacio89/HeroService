using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Gère l'écran Hero Collection (Heroes tab) : grille de héros parcourable
/// et filtrage par panthéon.
///
/// Les chips de filtre ("All", "Greek", "Norse", ...) sont construites
/// dynamiquement à partir des mythologyName présents dans le catalogue —
/// pas de taxonomie codée en dur côté client, pour rester aligné avec
/// la liste de panthéons gérée côté design/contenu.
///
/// Ce presenter ne contient AUCUNE logique de sélection de héros pour un
/// match — c'est un écran de consultation, distinct du flux HeroSelectionPanelPresenter.
/// </summary>
public class HeroCollectionPresenter : MonoBehaviour
{
    private const string AllFilterValue = "__all__";
    private const string AllFilterLabel = "All";

    [Header("Services")]
    [SerializeField] private HeroCatalogService heroCatalogService;
    [SerializeField] private HeroPortraitLibrary portraitLibrary;

    [Header("Grid")]
    [SerializeField] private Transform heroGridContainer;
    [SerializeField] private HeroCollectionItemView heroItemPrefab;

    [Header("Filters")]
    [SerializeField] private Transform filterChipContainer;
    [SerializeField] private PantheonFilterChipView filterChipPrefab;
    [SerializeField] private TMP_Text catalogStatusText;

    private readonly List<HeroCollectionItemView> spawnedItems = new();
    private readonly List<PantheonFilterChipView> spawnedChips = new();

    private HeroListItemDto[] allHeroes = System.Array.Empty<HeroListItemDto>();
    private string activeFilter = AllFilterValue;

    private void OnEnable()
    {
        if (heroCatalogService == null)
        {
            SetCatalogStatus("Hero catalogue is not configured.", true);
            Debug.LogWarning("[HeroCollectionPresenter] HeroCatalogService is missing.");
            return;
        }

        heroCatalogService.OnCatalogLoadStarted += HandleCatalogLoadStarted;
        heroCatalogService.OnCatalogLoaded += HandleCatalogLoaded;
        heroCatalogService.OnCatalogLoadFailed += HandleCatalogLoadFailed;

        if (heroCatalogService.HasLoaded)
        {
            HandleCatalogLoaded(heroCatalogService.CachedHeroes.ToArray());
        }
        else
        {
            SetCatalogStatus("Loading heroes…", true);
        }
    }

    private void OnDisable()
    {
        if (heroCatalogService != null)
        {
            heroCatalogService.OnCatalogLoadStarted -= HandleCatalogLoadStarted;
            heroCatalogService.OnCatalogLoaded -= HandleCatalogLoaded;
            heroCatalogService.OnCatalogLoadFailed -= HandleCatalogLoadFailed;
        }
    }

    private void HandleCatalogLoadStarted()
    {
        SetCatalogStatus("Loading heroes…", true);
    }

    private void HandleCatalogLoaded(HeroListItemDto[] heroes)
    {
        allHeroes = heroes ?? System.Array.Empty<HeroListItemDto>();
        activeFilter = AllFilterValue;

        BuildFilterChips();
        BuildHeroGrid();
    }

    private void HandleCatalogLoadFailed(string error)
    {
        allHeroes = System.Array.Empty<HeroListItemDto>();
        activeFilter = AllFilterValue;
        BuildFilterChips();
        BuildHeroGrid();
        SetCatalogStatus("Heroes are unavailable. Check your connection.", true);
        Debug.LogWarning($"[HeroCollectionPresenter] Hero catalogue load failed: {error}");
    }

    private void SetCatalogStatus(string message, bool visible)
    {
        if (catalogStatusText == null)
        {
            return;
        }

        catalogStatusText.text = message ?? string.Empty;
        catalogStatusText.gameObject.SetActive(visible);
    }

    private void BuildFilterChips()
    {
        ClearFilterChips();

        if (filterChipPrefab == null || filterChipContainer == null)
        {
            return;
        }

        List<string> mythologies = allHeroes
            .Where(h => h.affiliation != null && !string.IsNullOrWhiteSpace(h.affiliation.mythologyName))
            .Select(h => h.affiliation.mythologyName)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        CreateFilterChip(AllFilterLabel, AllFilterValue);

        foreach (string mythology in mythologies)
        {
            CreateFilterChip(mythology, mythology);
        }
    }

    private void CreateFilterChip(string label, string filterValue)
    {
        PantheonFilterChipView chip = Instantiate(filterChipPrefab, filterChipContainer);
        chip.Setup(label, filterValue, filterValue == activeFilter, OnFilterSelected);
        spawnedChips.Add(chip);
    }

    private void OnFilterSelected(string filterValue)
    {
        activeFilter = filterValue;

        // MVP V1 : reconstruction complète des chips à chaque changement de
        // filtre (même stratégie que BuildHeroList() dans
        // HeroSelectionPanelPresenter — pas d'optimisation prématurée).
        // BuildFilterChips() relit activeFilter pour ré-appliquer la sélection visuelle.
        BuildFilterChips();
        BuildHeroGrid();
    }

    private void BuildHeroGrid()
    {
        ClearHeroGrid();

        if (heroItemPrefab == null || heroGridContainer == null)
        {
            SetCatalogStatus("Hero collection is not configured.", true);
            return;
        }

        IEnumerable<HeroListItemDto> filtered = activeFilter == AllFilterValue
            ? allHeroes
            : allHeroes.Where(h => h.affiliation != null && h.affiliation.mythologyName == activeFilter);

        int renderedCount = 0;
        foreach (HeroListItemDto hero in filtered)
        {
            HeroCollectionItemView item = Instantiate(heroItemPrefab, heroGridContainer);
            Sprite portrait = portraitLibrary != null ? portraitLibrary.GetPortrait(hero.id) : null;
            item.Setup(hero, portrait, OnHeroClicked);
            spawnedItems.Add(item);
            renderedCount++;
        }

        if (renderedCount == 0)
        {
            string message = allHeroes.Length == 0
                ? "No heroes are available yet."
                : "No heroes match this pantheon.";
            SetCatalogStatus(message, true);
        }
        else
        {
            SetCatalogStatus(string.Empty, false);
        }
    }

    private void OnHeroClicked(string heroId)
    {
        // Réservé pour un futur panneau de détail héros (lore, skills, skins).
        Debug.Log($"[HeroCollectionPresenter] Hero selected for detail view: {heroId}");
    }

    private void ClearHeroGrid()
    {
        foreach (HeroCollectionItemView item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        spawnedItems.Clear();
    }

    private void ClearFilterChips()
    {
        foreach (PantheonFilterChipView chip in spawnedChips)
        {
            if (chip != null)
            {
                Destroy(chip.gameObject);
            }
        }

        spawnedChips.Clear();
    }
}
