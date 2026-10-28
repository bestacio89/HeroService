using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente une entrée visuelle unitaire de la liste de héros dans l'écran de sélection.
/// Cette vue reste passive : elle affiche le nom, l'état sélectionné
/// et relaie le clic utilisateur vers le presenter.
/// </summary>
public class HeroSelectionItemView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button button;

    private static readonly Color SurfaceColor = new(0.07f, 0.085f, 0.12f, 0.98f);
    private static readonly Color SelectedSurfaceColor = new(0.2f, 0.16f, 0.09f, 1f);
    private static readonly Color AccentColor = new(0.84f, 0.68f, 0.36f, 1f);

    private string heroId;
    private Action<string> onClick;
    private Image backgroundImage;
    private Image selectedIndicatorImage;
    private LayoutElement layoutElement;
    private Outline selectionOutline;

    public string HeroId => heroId;

    public void Setup(string heroId, string displayName, bool isSelected, Action<string> onClick)
    {
        this.heroId = heroId;
        this.onClick = onClick;

        if (heroNameText != null)
        {
            heroNameText.text = displayName;
            heroNameText.fontSize = 34f;
            heroNameText.fontStyle = TMPro.FontStyles.Bold;
            heroNameText.color = Color.white;
            heroNameText.alignment = TMPro.TextAlignmentOptions.Center;
            heroNameText.enableWordWrapping = false;
        }

        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(isSelected);
        }

        if (selectedIndicatorImage != null)
        {
            selectedIndicatorImage.color = new Color(AccentColor.r, AccentColor.g, AccentColor.b, 0.18f);
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? SelectedSurfaceColor : SurfaceColor;
        }

        if (selectionOutline != null)
        {
            selectionOutline.enabled = isSelected;
        }
    }

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = SurfaceColor;
            backgroundImage.raycastTarget = true;
        }

        layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }

        layoutElement.preferredHeight = 124f;
        layoutElement.minHeight = 112f;
        layoutElement.flexibleHeight = 0f;

        if (selectedIndicator != null)
        {
            selectedIndicatorImage = selectedIndicator.GetComponent<Image>();
            if (selectedIndicatorImage != null)
            {
                selectedIndicatorImage.raycastTarget = false;
            }
        }

        selectionOutline = GetComponent<Outline>();
        if (selectionOutline == null)
        {
            selectionOutline = gameObject.AddComponent<Outline>();
        }

        selectionOutline.effectColor = AccentColor;
        selectionOutline.effectDistance = new Vector2(3f, -3f);
        selectionOutline.useGraphicAlpha = true;
        selectionOutline.enabled = false;

        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.93f, 0.78f, 1f);
            colors.pressedColor = new Color(0.82f, 0.69f, 0.43f, 1f);
            colors.selectedColor = Color.white;
            colors.colorMultiplier = 1f;
            button.colors = colors;
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        onClick?.Invoke(heroId);
    }
}
