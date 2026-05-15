using System;
using MobaPrototype.Systems.Heroes;
using UnityEngine;

namespace MobaPrototype.Systems.Resources
{
    /// <summary>
    /// Handles mana for the current entity.
    /// Mana values are derived from HeroStatsProvider and maintained as runtime state.
    /// </summary>
    [RequireComponent(typeof(HeroStatsProvider))]
    /// <summary>
    /// Gère la ressource de mana et sa consommation.
    /// </summary>
    public class ManaSystem : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private bool restoreToFullManaOnStart = true;
        [SerializeField] private bool regenerateMana = true;
        [SerializeField] private float currentMana;

        private HeroStatsProvider heroStatsProvider;

        /// <summary>
        /// Raised whenever mana values change.
        /// Parameters are current mana and max mana.
        /// </summary>
        public event Action<float, float> OnManaChanged;

        /// <summary>
        /// Gets the current mana.
        /// </summary>
        public float CurrentMana => currentMana;

        /// <summary>
        /// Gets the maximum mana from the current hero stats.
        /// </summary>
        public float MaxMana => heroStatsProvider != null ? heroStatsProvider.MaxMana : 0f;

        private void Awake()
        {
            heroStatsProvider = GetComponent<HeroStatsProvider>();
        }

        private void OnEnable()
        {
            if (heroStatsProvider != null)
            {
                heroStatsProvider.OnStatsChanged += HandleStatsChanged;
            }
        }

        private void Start()
        {
            if (restoreToFullManaOnStart)
            {
                RestoreFullMana();
            }
            else
            {
                currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
                RaiseManaChanged();
            }
        }

        private void OnDisable()
        {
            if (heroStatsProvider != null)
            {
                heroStatsProvider.OnStatsChanged -= HandleStatsChanged;
            }
        }

        private void Update()
        {
            if (!regenerateMana)
            {
                return;
            }

            if (heroStatsProvider == null)
            {
                return;
            }

            float manaRegen = heroStatsProvider.ManaRegenPerSecond;

            if (manaRegen <= 0f || currentMana >= MaxMana)
            {
                return;
            }

            RestoreMana(manaRegen * Time.deltaTime);
        }

        /// <summary>
        /// Returns true if the entity has enough mana for the requested amount.
        /// </summary>
        /// <param name="amount">Required mana amount.</param>
        public bool HasEnoughMana(float amount)
        {
            if (amount <= 0f)
            {
                return true;
            }

            return currentMana >= amount;
        }

        /// <summary>
        /// Tries to spend mana.
        /// </summary>
        /// <param name="amount">Mana cost.</param>
        /// <returns>True if mana was successfully spent.</returns>
        public bool TrySpendMana(float amount)
        {
            if (amount <= 0f)
            {
                return true;
            }

            if (!HasEnoughMana(amount))
            {
                return false;
            }

            currentMana -= amount;
            currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
            RaiseManaChanged();
            return true;
        }

        /// <summary>
        /// Restores mana by a given amount.
        /// </summary>
        /// <param name="amount">Amount to restore.</param>
        public void RestoreMana(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            float previousMana = currentMana;
            currentMana = Mathf.Clamp(currentMana + amount, 0f, MaxMana);

            if (!Mathf.Approximately(previousMana, currentMana))
            {
                RaiseManaChanged();
            }
        }

        /// <summary>
        /// Restores mana to its current maximum.
        /// </summary>
        public void RestoreFullMana()
        {
            currentMana = MaxMana;
            RaiseManaChanged();
        }

        private void HandleStatsChanged()
        {
            currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
            RaiseManaChanged();
        }

        private void RaiseManaChanged()
        {
            OnManaChanged?.Invoke(currentMana, MaxMana);
        }
    }
}