using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Offsets edge-anchored HUD elements away from device cutouts and gesture
/// areas without changing their authored anchors or size.
/// Use SafeAreaController instead for a full-screen root.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SafeAreaEdgePadding : MonoBehaviour
{
    private RectTransform target;
    private Vector2 authoredPosition;
    private Vector2 authoredAnchorMin;
    private Vector2 authoredAnchorMax;
    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private float lastScaleFactor = -1f;

    private void Awake()
    {
        target = GetComponent<RectTransform>();
        authoredPosition = target.anchoredPosition;
        authoredAnchorMin = target.anchorMin;
        authoredAnchorMax = target.anchorMax;
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
        if (target == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        float scaleFactor = GetCanvasScaleFactor();
        Rect safeArea = Screen.safeArea;
        float left = safeArea.xMin / scaleFactor;
        float right = (Screen.width - safeArea.xMax) / scaleFactor;
        float bottom = safeArea.yMin / scaleFactor;
        float top = (Screen.height - safeArea.yMax) / scaleFactor;

        Vector2 offset = Vector2.zero;
        if (IsAtStart(authoredAnchorMin.x) && IsAtStart(authoredAnchorMax.x))
        {
            offset.x += left;
        }
        else if (IsAtEnd(authoredAnchorMin.x) && IsAtEnd(authoredAnchorMax.x))
        {
            offset.x -= right;
        }

        if (IsAtStart(authoredAnchorMin.y) && IsAtStart(authoredAnchorMax.y))
        {
            offset.y += bottom;
        }
        else if (IsAtEnd(authoredAnchorMin.y) && IsAtEnd(authoredAnchorMax.y))
        {
            offset.y -= top;
        }

        target.anchoredPosition = authoredPosition + offset;
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

    private static bool IsAtStart(float value) => value <= 0.001f;
    private static bool IsAtEnd(float value) => value >= 0.999f;
}
