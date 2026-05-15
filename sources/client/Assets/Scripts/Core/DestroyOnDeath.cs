using UnityEngine;
using MobaPrototype.Systems.Health;

/// <summary>
/// Disables a GameObject when its HealthSystem triggers a death event.
/// </summary>
[RequireComponent(typeof(HealthSystem))]
/// <summary>
/// Détruit l'objet quand la mort du porteur est détectée.
/// </summary>
public class DestroyOnDeath : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private GameObject objectToDisable;

    private void Awake()
    {
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();
        }

        if (objectToDisable == null)
        {
            objectToDisable = gameObject;
        }
    }

    private void OnEnable()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDied += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDied -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
    }
}