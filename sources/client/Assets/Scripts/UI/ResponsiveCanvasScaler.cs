using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps menu sizing legible across portrait and landscape aspect ratios.
/// A portrait reference is used for portrait screens and a landscape
/// reference for landscape screens; CanvasScaler remains responsible for
/// the actual scale calculation.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasScaler))]
public sealed class ResponsiveCanvasScaler : MonoBehaviour
{
    private static readonly Vector2 PortraitReference = new(1080f, 1920f);
    private static readonly Vector2 LandscapeReference = new(1920f, 1080f);

    [SerializeField, Range(0f, 1f)]
    private float portraitMatchWidthOrHeight = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float landscapeMatchWidthOrHeight = 0.5f;

    private CanvasScaler scaler;
    private bool lastLandscape;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        ApplyOrientation(force: true);
    }

    private void Update()
    {
        ApplyOrientation(force: false);
    }

    private void ApplyOrientation(bool force)
    {
        if (scaler == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        bool isLandscape = Screen.width > Screen.height;
        if (!force && isLandscape == lastLandscape)
        {
            return;
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = isLandscape ? LandscapeReference : PortraitReference;
        scaler.matchWidthOrHeight = isLandscape
            ? landscapeMatchWidthOrHeight
            : portraitMatchWidthOrHeight;

        lastLandscape = isLandscape;
    }
}
