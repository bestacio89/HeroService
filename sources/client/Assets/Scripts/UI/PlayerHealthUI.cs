using UnityEngine;
using UnityEngine.UI;
using MobaPrototype.Systems.Health;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Updates the player health bar UI from a HealthSystem.
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthSystem playerHealthSystem;
        [SerializeField] private Image healthFillImage;

        private void Awake()
        {
            if (playerHealthSystem == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerHealthSystem = player.GetComponent<HealthSystem>();
                }
            }
        }

        private void OnEnable()
        {
            if (playerHealthSystem != null)
            {
                playerHealthSystem.OnHealthChanged += UpdateHealthBar;
            }
        }

        private void Start()
        {
            if (playerHealthSystem != null)
            {
                UpdateHealthBar(playerHealthSystem.CurrentHealth, playerHealthSystem.MaxHealth);
            }
        }

        private void OnDisable()
        {
            if (playerHealthSystem != null)
            {
                playerHealthSystem.OnHealthChanged -= UpdateHealthBar;
            }
        }

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthFillImage == null || maxHealth <= 0f)
            {
                return;
            }

            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}