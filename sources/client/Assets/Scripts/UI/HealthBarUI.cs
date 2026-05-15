using UnityEngine;
using UnityEngine.UI;
using MobaPrototype.Systems.Health;

/// <summary>
/// Handles the world-space health bar display for a target entity.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthSystem targetHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.5f, 0f);
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private Camera mainCamera;

    [Header("Visual Root")]
    [SerializeField] private GameObject visualRoot;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateBar;
            targetHealth.OnDied += HideBar;
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateBar;
            targetHealth.OnDied -= HideBar;
        }
    }

    private void Start()
    {
        if (targetHealth != null)
        {
            UpdateBar(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        }
    }

    private void LateUpdate()
    {
        if (targetToFollow != null)
        {
            transform.position = targetToFollow.position + worldOffset;
        }

        if (mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }
    }

    public void UpdateBar(float current, float max)
    {
        if (fillImage == null)
        {
            return;
        }

        if (max <= 0f)
        {
            return;
        }

        fillImage.fillAmount = current / max;

        if (visualRoot != null)
        {
            visualRoot.SetActive(current > 0f);
        }
    }

    private void HideBar()
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }
    }
}