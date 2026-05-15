using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Implements a virtual joystick used for mobile input.
/// 
/// This component detects pointer interactions (touch or mouse)
/// and converts them into a normalized directional vector.
/// 
/// The joystick consists of:
/// - A background (movement boundary)
/// - A handle (movable stick)
/// 
/// The resulting direction can be read through the <see cref="Direction"/> property
/// and used by gameplay systems such as player movement.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    /// <summary>
    /// Background area of the joystick.
    /// Defines the movement boundary for the handle.
    /// </summary>
    [SerializeField] private RectTransform background;

    /// <summary>
    /// The movable joystick handle.
    /// Its position reflects the current input direction.
    /// </summary>
    [SerializeField] private RectTransform handle;

    /// <summary>
    /// Maximum distance the handle can move from the center.
    /// Controls the visual range of the joystick.
    /// </summary>
    [SerializeField] private float handleRange = 80f;

    /// <summary>
    /// Current normalized direction of the joystick input.
    /// Values range from -1 to 1 on both axes.
    /// </summary>
    public Vector2 Direction { get; private set; }

    /// <summary>
    /// Reference to the canvas containing this joystick.
    /// Used to determine which camera processes UI input.
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// Camera used for UI raycasting when the canvas is not in Overlay mode.
    /// </summary>
    private Camera uiCamera;

    /// <summary>
    /// Initializes references to the canvas and UI camera.
    /// </summary>
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        // If the canvas is not overlay, UI input requires a camera reference
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }
    }

    /// <summary>
    /// Called when the pointer first touches the joystick.
    /// Immediately forwards the event to the drag handler.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    /// <summary>
    /// Called while the pointer is dragging the joystick.
    /// 
    /// Converts the screen pointer position into a local position
    /// inside the joystick background, then calculates the direction
    /// and moves the handle accordingly.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            uiCamera,
            out localPoint))
        {
            Vector2 radius = background.sizeDelta * 0.5f;

            // Normalize input relative to joystick radius
            Direction = new Vector2(localPoint.x / radius.x, localPoint.y / radius.y);

            // Clamp direction to a maximum magnitude of 1
            Direction = Vector2.ClampMagnitude(Direction, 1f);

            // Move the handle visually
            handle.anchoredPosition = Direction * handleRange;
        }
    }

    /// <summary>
    /// Called when the pointer is released.
    /// Resets the joystick to its neutral position.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }
}