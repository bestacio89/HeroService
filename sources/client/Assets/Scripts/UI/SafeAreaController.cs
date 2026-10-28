using UnityEngine;

/// <summary>
/// Insets a full-screen UI RectTransform into the device's safe area.
/// Attach to a full-screen UI root, not to the Canvas itself.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SafeAreaController : MonoBehaviour
{
    private RectTransform target;
    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        target = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (target == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;
        target.anchorMin = new Vector2(
            safeArea.xMin / Screen.width,
            safeArea.yMin / Screen.height);
        target.anchorMax = new Vector2(
            safeArea.xMax / Screen.width,
            safeArea.yMax / Screen.height);
        target.offsetMin = Vector2.zero;
        target.offsetMax = Vector2.zero;

        lastSafeArea = safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}
