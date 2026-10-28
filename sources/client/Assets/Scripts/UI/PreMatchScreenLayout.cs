using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rebuilds the existing pre-match panels into mobile-safe, scrollable layouts
/// without replacing the serialized presenter or button references.
/// </summary>
[DisallowMultipleComponent]
public sealed class PreMatchScreenLayout : MonoBehaviour
{
    private static readonly Color BackgroundColor = new(0.035f, 0.045f, 0.07f, 1f);
    private static readonly Color SurfaceColor = new(0.07f, 0.085f, 0.12f, 0.98f);
    private static readonly Color AccentColor = new(0.84f, 0.68f, 0.36f, 1f);
    private static readonly Color MutedTextColor = new(0.72f, 0.76f, 0.82f, 1f);

    private RectTransform modeColumn;
    private RectTransform modeSafeArea;
    private float lastModeAreaWidth = -1f;

    private void Awake()
    {
        Transform modePanel = FindDescendant(transform, "ModeSelectionPanel");
        Transform heroPanel = FindDescendant(transform, "HeroSelectionPanel");

        if (modePanel != null)
        {
            ConfigureModeSelection(modePanel);
        }

        if (heroPanel != null)
        {
            ConfigureHeroSelection(heroPanel);
        }

        UpdateModeColumnWidth(force: true);
    }

    private void LateUpdate()
    {
        UpdateModeColumnWidth(force: false);
    }

    private void ConfigureModeSelection(Transform panel)
    {
        Image panelBackground = GetOrAddComponent<Image>(panel.gameObject);
        panelBackground.color = BackgroundColor;
        panelBackground.raycastTarget = false;

        Transform safeArea = EnsureSafeAreaRoot(panel, "ModeSelectionSafeArea");
        modeSafeArea = safeArea.GetComponent<RectTransform>();

        Transform column = FindDescendant(safeArea, "ModeSelectionColumn");
        if (column == null)
        {
            column = CreateRect("ModeSelectionColumn", safeArea);
        }
        else if (column.parent != safeArea)
        {
            column.SetParent(safeArea, false);
        }

        modeColumn = column.GetComponent<RectTransform>();
        modeColumn.anchorMin = new Vector2(0.5f, 0f);
        modeColumn.anchorMax = new Vector2(0.5f, 1f);
        modeColumn.pivot = new Vector2(0.5f, 0.5f);
        modeColumn.anchoredPosition = Vector2.zero;
        modeColumn.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = GetOrAddComponent<VerticalLayoutGroup>(column.gameObject);
        layout.padding = new RectOffset(32, 32, 64, 64);
        layout.spacing = 24f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TMP_Text title = EnsureText(column, "ModeSelectionTitle", "CHOOSE YOUR MODE", 56);
        RectTransform titleLayoutRect = title.GetComponent<RectTransform>();
        titleLayoutRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleLayoutRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleLayoutRect.pivot = new Vector2(0.5f, 0.5f);
        title.color = AccentColor;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.enableWordWrapping = false;
        SetLayoutHeight(title.gameObject, 100f);
        title.transform.SetSiblingIndex(0);

        TMP_Text subtitle = EnsureText(column, "ModeSelectionSubtitle", "Select your battlefield", 32);
        RectTransform subtitleLayoutRect = subtitle.GetComponent<RectTransform>();
        subtitleLayoutRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleLayoutRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleLayoutRect.pivot = new Vector2(0.5f, 0.5f);
        subtitle.color = MutedTextColor;
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.enableWordWrapping = false;
        SetLayoutHeight(subtitle.gameObject, 60f);
        subtitle.transform.SetSiblingIndex(1);

        ConfigureModeButton(panel, column, "Button_Classic", "CLASSIC", 2);
        ConfigureModeButton(panel, column, "Button_Ranked", "RANKED", 3);
        ConfigureModeButton(panel, column, "Button_Arcade", "ARCADE", 4);
    }

    private void ConfigureModeButton(
        Transform panel,
        Transform column,
        string buttonName,
        string label,
        int siblingIndex)
    {
        Transform buttonTransform = FindDescendant(panel, buttonName);
        if (buttonTransform == null)
        {
            Debug.LogError($"[PreMatchScreenLayout] Required mode button '{buttonName}' was not found.");
            return;
        }

        buttonTransform.SetParent(column, false);
        RectTransform rect = buttonTransform.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(0f, 112f);
        rect.anchoredPosition = Vector2.zero;
        SetLayoutHeight(buttonTransform.gameObject, 112f, 96f);

        Image background = GetOrAddComponent<Image>(buttonTransform.gameObject);
        background.color = SurfaceColor;
        background.raycastTarget = true;

        Button button = GetOrAddComponent<Button>(buttonTransform.gameObject);
        ApplyButtonColors(button, SurfaceColor, AccentColor);

        TMP_Text labelText = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (labelText == null)
        {
            labelText = EnsureText(buttonTransform, "Label", label, 36);
        }

        labelText.text = label;
        labelText.fontSize = 40f;
        labelText.fontStyle = FontStyles.Bold;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = Color.white;
        labelText.enableWordWrapping = false;
        buttonTransform.SetSiblingIndex(siblingIndex);
    }

    private void ConfigureHeroSelection(Transform panel)
    {
        Image panelBackground = GetOrAddComponent<Image>(panel.gameObject);
        panelBackground.color = BackgroundColor;
        panelBackground.raycastTarget = false;

        Transform safeArea = EnsureSafeAreaRoot(panel, "HeroSelectionSafeArea");
        Transform titleTransform = FindDescendant(panel, "HeroSelectionTitle");
        Transform listTransform = FindDescendant(panel, "HeroListContainer");
        Transform confirmTransform = FindDescendant(panel, "ConfirmButton");

        if (titleTransform == null || listTransform == null || confirmTransform == null)
        {
            Debug.LogError("[PreMatchScreenLayout] Hero selection is missing its title, list, or confirmation button.");
            return;
        }

        titleTransform.SetParent(safeArea, false);
        RectTransform titleRect = titleTransform.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -24f);
        titleRect.sizeDelta = new Vector2(1000f, 100f);

        TMP_Text title = titleTransform.GetComponent<TMP_Text>();
        if (title != null)
        {
            title.text = "CHOOSE YOUR HERO";
            title.fontSize = 56f;
            title.fontStyle = FontStyles.Bold;
            title.color = AccentColor;
            title.alignment = TextAlignmentOptions.Center;
            title.enableWordWrapping = false;
        }

        Transform scrollView = FindDescendant(safeArea, "HeroListScrollView");
        if (scrollView == null)
        {
            scrollView = CreateRect("HeroListScrollView", safeArea);
        }

        RectTransform scrollRectTransform = scrollView.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0.04f, 0.17f);
        scrollRectTransform.anchorMax = new Vector2(0.96f, 0.78f);
        scrollRectTransform.offsetMin = Vector2.zero;
        scrollRectTransform.offsetMax = Vector2.zero;

        Image scrollSurface = GetOrAddComponent<Image>(scrollView.gameObject);
        scrollSurface.color = new Color(0f, 0f, 0f, 0f);
        scrollSurface.raycastTarget = true;

        ScrollRect scrollRect = GetOrAddComponent<ScrollRect>(scrollView.gameObject);
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.scrollSensitivity = 35f;

        Transform viewport = FindDescendant(scrollView, "Viewport");
        if (viewport == null)
        {
            viewport = CreateRect("Viewport", scrollView);
        }

        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportSurface = GetOrAddComponent<Image>(viewport.gameObject);
        viewportSurface.color = new Color(0f, 0f, 0f, 0f);
        viewportSurface.raycastTarget = true;
        GetOrAddComponent<RectMask2D>(viewport.gameObject);

        listTransform.SetParent(viewport, false);
        RectTransform listRect = listTransform.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 1f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.pivot = new Vector2(0.5f, 1f);
        listRect.anchoredPosition = Vector2.zero;
        listRect.sizeDelta = Vector2.zero;

        VerticalLayoutGroup listLayout = GetOrAddComponent<VerticalLayoutGroup>(listTransform.gameObject);
        listLayout.padding = new RectOffset(12, 12, 12, 12);
        listLayout.spacing = 16f;
        listLayout.childAlignment = TextAnchor.UpperCenter;
        listLayout.childControlWidth = true;
        listLayout.childControlHeight = false;
        listLayout.childForceExpandWidth = true;
        listLayout.childForceExpandHeight = false;

        ContentSizeFitter fitter = GetOrAddComponent<ContentSizeFitter>(listTransform.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRect;
        scrollRect.content = listRect;
        scrollView.SetSiblingIndex(1);

        confirmTransform.SetParent(safeArea, false);
        RectTransform confirmRect = confirmTransform.GetComponent<RectTransform>();
        confirmRect.anchorMin = new Vector2(0.5f, 0f);
        confirmRect.anchorMax = new Vector2(0.5f, 0f);
        confirmRect.pivot = new Vector2(0.5f, 0f);
        confirmRect.anchoredPosition = new Vector2(0f, 24f);
        confirmRect.sizeDelta = new Vector2(460f, 100f);
        SetLayoutHeight(confirmTransform.gameObject, 100f, 96f);
        LayoutElement confirmLayout = GetOrAddComponent<LayoutElement>(confirmTransform.gameObject);
        confirmLayout.preferredWidth = 460f;
        confirmLayout.minWidth = 280f;

        Image confirmBackground = GetOrAddComponent<Image>(confirmTransform.gameObject);
        confirmBackground.color = AccentColor;
        confirmBackground.raycastTarget = true;

        Button confirmButton = GetOrAddComponent<Button>(confirmTransform.gameObject);
        ApplyButtonColors(confirmButton, AccentColor, Color.white);

        TMP_Text confirmLabel = confirmTransform.GetComponentInChildren<TMP_Text>(true);
        if (confirmLabel != null)
        {
            confirmLabel.text = "LOCK IN";
            confirmLabel.fontSize = 40f;
            confirmLabel.fontStyle = FontStyles.Bold;
            confirmLabel.color = BackgroundColor;
            confirmLabel.alignment = TextAlignmentOptions.Center;
            confirmLabel.enableWordWrapping = false;
        }

        titleTransform.SetSiblingIndex(0);
        scrollView.SetSiblingIndex(1);
        confirmTransform.SetAsLastSibling();
    }

    private void UpdateModeColumnWidth(bool force)
    {
        if (modeColumn == null || modeSafeArea == null)
        {
            return;
        }

        float availableWidth = modeSafeArea.rect.width;
        if (availableWidth <= 0f || (!force && Mathf.Abs(availableWidth - lastModeAreaWidth) < 1f))
        {
            return;
        }

        float width = Mathf.Min(900f, availableWidth * 0.9f);
        modeColumn.sizeDelta = new Vector2(width, 0f);
        lastModeAreaWidth = availableWidth;
    }

    private static Transform EnsureSafeAreaRoot(Transform panel, string name)
    {
        Transform safeArea = FindDescendant(panel, name);
        if (safeArea == null)
        {
            safeArea = CreateRect(name, panel);
        }
        else if (safeArea.parent != panel)
        {
            safeArea.SetParent(panel, false);
        }

        RectTransform rect = safeArea.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        GetOrAddComponent<SafeAreaController>(safeArea.gameObject);
        return safeArea;
    }

    private static TMP_Text EnsureText(Transform parent, string name, string value, float fontSize)
    {
        Transform existing = parent.Find(name);
        GameObject textObject = existing != null ? existing.gameObject : CreateRect(name, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TMP_Text text = GetOrAddComponent<TextMeshProUGUI>(textObject);
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        return text;
    }

    private static void SetLayoutHeight(GameObject target, float preferredHeight, float minimumHeight = -1f)
    {
        LayoutElement layout = GetOrAddComponent<LayoutElement>(target);
        layout.preferredHeight = preferredHeight;
        if (minimumHeight >= 0f)
        {
            layout.minHeight = minimumHeight;
        }
    }

    private static void ApplyButtonColors(Button button, Color baseColor, Color accent)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.Lerp(Color.white, accent, 0.2f);
        colors.pressedColor = Color.Lerp(baseColor, accent, 0.35f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.55f, 0.57f, 0.6f, 0.6f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform nested = FindDescendant(child, name);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static Transform CreateRect(string name, Transform parent)
    {
        GameObject child = new(name, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child.transform;
    }

    private static T GetOrAddComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
    }

}
