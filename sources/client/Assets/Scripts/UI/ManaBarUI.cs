using UnityEngine;
using UnityEngine.UI;
using MobaPrototype.Systems.Resources;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Displays the player's mana in the UI.
    /// Listens to ManaSystem events and updates the fill image.
    /// This script contains no gameplay logic.
    /// </summary>
    public sealed class ManaBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ManaSystem manaSystem;
        [SerializeField] private Image manaFillImage;

        private void OnEnable()
        {
            if (manaSystem != null)
            {
                manaSystem.OnManaChanged += HandleManaChanged;
            }
        }

        private void Start()
        {
            if (manaSystem != null)
            {
                UpdateBar(manaSystem.CurrentMana, manaSystem.MaxMana);
            }
        }

        private void OnDisable()
        {
            if (manaSystem != null)
            {
                manaSystem.OnManaChanged -= HandleManaChanged;
            }
        }

        private void HandleManaChanged(float currentMana, float maxMana)
        {
            UpdateBar(currentMana, maxMana);
        }

        private void UpdateBar(float currentMana, float maxMana)
        {
            if (manaFillImage == null || maxMana <= 0f)
            {
                return;
            }

            manaFillImage.fillAmount = currentMana / maxMana;
        }
    }
}