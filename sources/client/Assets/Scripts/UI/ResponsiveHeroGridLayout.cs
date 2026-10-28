using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sizes hero cards for the available viewport instead of using a fixed card
/// size that becomes too small on phones or wastes space on larger displays.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(GridLayoutGroup))]
public sealed class ResponsiveHeroGridLayout : MonoBehaviour
{
    [SerializeField, Min(1)] private int maximumColumns = 5;
    [SerializeField, Min(1f)] private float minimumCellWidth = 280f;
    [SerializeField, Min(1f)] private float maximumCellWidth = 420f;
    [SerializeField, Min(0.5f)] private float cellHeightToWidthRatio = 1.25f;

    private GridLayoutGroup grid;
    private RectTransform viewport;
    private float lastWidth = -1f;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        viewport = transform.parent as RectTransform;
        Recalculate(force: true);
    }

    private void OnEnable()
    {
        Recalculate(force: true);
    }

    private void LateUpdate()
    {
        Recalculate(force: false);
    }

    private void Recalculate(bool force)
    {
        if (grid == null || viewport == null)
        {
            return;
        }

        float width = viewport.rect.width;
        if (width <= 0f || (!force && Mathf.Abs(width - lastWidth) < 0.5f))
        {
            return;
        }

        RectOffset padding = grid.padding;
        float availableWidth = Mathf.Max(1f, width - padding.left - padding.right);
        float horizontalSpacing = grid.spacing.x;
        int columnsForMinimum = Mathf.Max(
            1,
            Mathf.FloorToInt((availableWidth + horizontalSpacing) / (minimumCellWidth + horizontalSpacing)));
        int columnsForMaximum = Mathf.Max(
            1,
            Mathf.CeilToInt((availableWidth + horizontalSpacing) / (maximumCellWidth + horizontalSpacing)));
        int columns = Mathf.Clamp(
            Mathf.Min(columnsForMinimum, maximumColumns),
            columnsForMaximum,
            maximumColumns);

        float cellWidth = (availableWidth - horizontalSpacing * (columns - 1)) / columns;
        grid.cellSize = new Vector2(cellWidth, cellWidth * cellHeightToWidthRatio);
        lastWidth = width;
    }
}
