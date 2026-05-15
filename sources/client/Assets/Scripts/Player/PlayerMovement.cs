using MobaPrototype.Systems.Heroes;
using MobaPrototype.Systems.Health;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles top-down player movement using a CharacterController.
///
/// This component supports both keyboard input and virtual joystick input,
/// converts movement into camera-relative direction, applies gravity,
/// and smoothly rotates the player toward the movement direction.
///
/// Movement speed can optionally come from HeroStatsProvider.
/// If no HeroStatsProvider is found, the local fallback moveSpeed value is used.
///
/// Requirements:
/// - A CharacterController on the same GameObject
/// - An optional VirtualJoystick for mobile input
/// - An optional camera reference for camera-relative movement
/// - An optional HeroStatsProvider on the same GameObject
/// - An optional PlayerInputGate on the same GameObject
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthSystem))]
/// <summary>
/// Gère les déplacements du héros contrôlé par le joueur.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float inputDeadZone = 0.1f;

    [Header("References")]
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private HeroStatsProvider heroStatsProvider;
    [SerializeField] private PlayerInputGate inputGate;

    private HealthSystem healthSystem;
    private Vector3 verticalVelocity;

    /// <summary>
    /// Gets the effective movement speed currently used by the player.
    /// Uses hero stats when available, otherwise falls back to the local moveSpeed value.
    /// </summary>
    public float CurrentMoveSpeed
    {
        get
        {
            if (heroStatsProvider != null)
            {
                return Mathf.Max(0f, heroStatsProvider.MovementSpeed);
            }

            return Mathf.Max(0f, moveSpeed);
        }
    }

    /// <summary>
    /// Called when the component is first added or reset in the Inspector.
    /// Automatically assigns local component references when possible.
    /// </summary>
    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
        heroStatsProvider = GetComponent<HeroStatsProvider>();
        inputGate = GetComponent<PlayerInputGate>();
    }

    /// <summary>
    /// Initializes missing references at runtime.
    /// - Retrieves the CharacterController if not assigned
    /// - Retrieves HealthSystem
    /// - Retrieves HeroStatsProvider if present
    /// - Retrieves PlayerInputGate if present
    /// - Uses the main camera if no camera transform is set
    /// </summary>
    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        healthSystem = GetComponent<HealthSystem>();

        if (heroStatsProvider == null)
        {
            heroStatsProvider = GetComponent<HeroStatsProvider>();
        }

        if (inputGate == null)
        {
            inputGate = GetComponent<PlayerInputGate>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        verticalVelocity = new Vector3(0f, -2f, 0f);
    }

    private void Update()
    {
        if (characterController == null || !characterController.enabled)
        {
            return;
        }

        if (healthSystem != null && healthSystem.IsDead)
        {
            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            return;
        }

        if (inputGate != null && !inputGate.IsGameplayInputAllowed())
        {
            StopMovement();
            ApplyGravityOnly();
            return;
        }

        Vector2 input = ReadMovementInput();

        if (input.sqrMagnitude < inputDeadZone * inputDeadZone)
        {
            input = Vector2.zero;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Vector3 moveDirection = GetCameraRelativeDirection(input);

        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = moveDirection * CurrentMoveSpeed;
        finalMove.y = verticalVelocity.y;

        characterController.Move(finalMove * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Reads movement input from both keyboard and virtual joystick.
    ///
    /// Keyboard:
    /// - WASD
    /// - Arrow keys
    ///
    /// Mobile:
    /// - Virtual joystick if assigned
    ///
    /// Returns a clamped 2D input vector with a maximum magnitude of 1.
    /// </summary>
    /// <returns>The final normalized movement input.</returns>
    private Vector2 ReadMovementInput()
    {
        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                keyboardInput.y += 1f;
            }

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                keyboardInput.y -= 1f;
            }

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                keyboardInput.x -= 1f;
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                keyboardInput.x += 1f;
            }
        }

        Vector2 joystickInput = joystick != null ? joystick.Direction : Vector2.zero;
        Vector2 finalInput = keyboardInput + joystickInput;

        return Vector2.ClampMagnitude(finalInput, 1f);
    }

    /// <summary>
    /// Converts 2D input into a world-space movement direction relative to the camera.
    ///
    /// If no camera is assigned, movement falls back to world axes:
    /// - X = horizontal
    /// - Z = vertical
    /// </summary>
    /// <param name="input">Raw 2D movement input.</param>
    /// <returns>A normalized 3D movement direction.</returns>
    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (cameraTransform == null)
        {
            return new Vector3(input.x, 0f, input.y).normalized;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * input.y + right * input.x;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        return direction;
    }

    private void StopMovement()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }
    }

    private void ApplyGravityOnly()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 gravityMove = new Vector3(0f, verticalVelocity.y, 0f);
        characterController.Move(gravityMove * Time.deltaTime);
    }

    /// <summary>
    /// Appelé par HeroSpawner pour câbler le joystick sur le joueur local.
    /// </summary>
    public void SetJoystick(VirtualJoystick virtualJoystick)
    {
        joystick = virtualJoystick;
    }
}