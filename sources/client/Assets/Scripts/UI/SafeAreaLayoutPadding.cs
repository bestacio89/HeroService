using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds device safe-area insets to an existing Unity UI LayoutGroup without
/// changing its anchors or interfering with ContentSizeFitter sizing.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(LayoutGroup))]
public sealed class SafeAreaLayoutPadding : MonoBehaviour
{
    private LayoutGroup layoutGroup;
    private RectOffset authoredPadding;
    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private float lastScaleFactor = -1f;

    private void Awake()
    {
        layoutGroup = GetComponent<LayoutGroup>();
        RectOffset current = layoutGroup.padding;
        authoredPadding = new RectOffset(current.left, current.right, current.top, current.bottom);
        ApplyInsets();
    }

    private void OnEnable()
    {
        ApplyInsets();
    }

    private void Update()
    {
        float scaleFactor = GetCanvasScaleFactor();
        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            Screen.safeArea != lastSafeArea ||
            !Mathf.Approximately(scaleFactor, lastScaleFactor))
        {
            ApplyInsets();
        }
    }

    private void ApplyInsets()
    {
        if (layoutGroup == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        float scaleFactor = GetCanvasScaleFactor();
        Rect safeArea = Screen.safeArea;
        int left = Mathf.CeilToInt(safeArea.xMin / scaleFactor);
        int right = Mathf.CeilToInt((Screen.width - safeArea.xMax) / scaleFactor);
        int bottom = Mathf.CeilToInt(safeArea.yMin / scaleFactor);
        int top = Mathf.CeilToInt((Screen.height - safeArea.yMax) / scaleFactor);

        layoutGroup.padding = new RectOffset(
            authoredPadding.left + left,
            authoredPadding.right + right,
            authoredPadding.top + top,
            authoredPadding.bottom + bottom);

        lastSafeArea = safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastScaleFactor = scaleFactor;
    }

    private float GetCanvasScaleFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Canvas rootCanvas = canvas != null ? canvas.rootCanvas : null;
        CanvasScaler scaler = rootCanvas != null ? rootCanvas.GetComponent<CanvasScaler>() : null;
        return scaler != null && scaler.scaleFactor > 0f ? scaler.scaleFactor : 1f;
    }
}
