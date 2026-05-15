using UnityEngine;

/// <summary>
/// Handles the visual highlighting of an enemy when it becomes the player's current target.
/// 
/// This component checks if the GameObject is the active target selected by
/// <see cref="PlayerTargeting"/> and changes its renderer color accordingly.
/// 
/// Typical behavior:
/// - Default color when the enemy is not targeted
/// - Target color when the enemy is the current target
/// 
/// This provides a simple visual feedback to help the player identify
/// which enemy is currently selected.
/// </summary>
public class TargetHighlight : MonoBehaviour
{
    /// <summary>
    /// Reference to the player's targeting system.
    /// Used to determine which enemy is currently targeted.
    /// </summary>
    [SerializeField] private PlayerTargeting targeting;

    /// <summary>
    /// Default color applied when the object is not the current target.
    /// </summary>
    [SerializeField] private Color defaultColor = Color.white;

    /// <summary>
    /// Color applied when the object is the player's current target.
    /// </summary>
    [SerializeField] private Color targetColor = Color.red;

    /// <summary>
    /// Cached list of renderers used to apply color changes efficiently.
    /// </summary>
    private Renderer[] cachedRenderers;

    /// <summary>
    /// Initializes references and caches the renderers of the object and its children.
    /// Also sets the default color at startup.
    /// </summary>
    private void Awake()
    {
        if (targeting == null)
            targeting = FindFirstObjectByType<PlayerTargeting>();

        cachedRenderers = GetComponentsInChildren<Renderer>();
        SetColor(defaultColor);
    }

    /// <summary>
    /// Checks every frame if this object is the current player target.
    /// If so, applies the target highlight color.
    /// Otherwise, restores the default color.
    /// </summary>
    private void Update()
    {
        bool isTargeted = false;

        if (targeting != null && targeting.CurrentTarget == transform)
            isTargeted = true;

        SetColor(isTargeted ? targetColor : defaultColor);
    }

    /// <summary>
    /// Applies a color to all cached renderers on this object and its children.
    /// </summary>
    /// <param name="color">The color to apply.</param>
    private void SetColor(Color color)
    {
        foreach (Renderer r in cachedRenderers)
        {
            if (r.material != null)
                r.material.color = color;
        }
    }
}