using System;
using MobaPrototype.Systems.Progression;
using UnityEngine;

namespace MobaPrototype.Systems.Heroes
{
    /// <summary>
    /// Calculates the final hero stats from the assigned hero definition and current level.
    /// This component is read-only for other systems and does not apply gameplay effects directly.
    /// </summary>
    [RequireComponent(typeof(HeroRuntime))]
    [RequireComponent(typeof(LevelSystem))]
    /// <summary>
    /// Expose les statistiques dérivées du héros courant.
    /// </summary>
    public class HeroStatsProvider : MonoBehaviour
    {
        private HeroRuntime heroRuntime;
        private LevelSystem levelSystem;

        /// <summary>
        /// Raised when final stats may have changed.
        /// </summary>
        public event Action OnStatsChanged;

        /// <summary>
        /// Gets the current hero definition.
        /// </summary>
        public HeroDefinition HeroDefinition => heroRuntime != null ? heroRuntime.HeroDefinition : null;

        /// <summary>
        /// Gets the current hero level.
        /// </summary>
        public int CurrentLevel => levelSystem != null ? levelSystem.CurrentLevel : 1;

        /// <summary>
        /// Gets the final maximum health for the current hero and level.
        /// </summary>
        public float MaxHealth => GetScaledStat(
            HeroDefinition != null ? HeroDefinition.BaseMaxHealth : 0f,
            HeroDefinition != null ? HeroDefinition.MaxHealthPerLevel : 0f);

        /// <summary>
        /// Gets the final maximum mana for the current hero and level.
        /// </summary>
        public float MaxMana => GetScaledStat(
            HeroDefinition != null ? HeroDefinition.BaseMaxMana : 0f,
            HeroDefinition != null ? HeroDefinition.MaxManaPerLevel : 0f);

        /// <summary>
        /// Gets the final attack damage for the current hero and level.
        /// </summary>
        public float AttackDamage => GetScaledStat(
            HeroDefinition != null ? HeroDefinition.BaseAttackDamage : 0f,
            HeroDefinition != null ? HeroDefinition.AttackDamagePerLevel : 0f);

        /// <summary>
        /// Gets the final movement speed for the current hero and level.
        /// </summary>
        public float MovementSpeed => GetScaledStat(
            HeroDefinition != null ? HeroDefinition.BaseMovementSpeed : 0f,
            HeroDefinition != null ? HeroDefinition.MovementSpeedPerLevel : 0f);

        /// <summary>
        /// Gets the final mana regeneration value for the current hero and level.
        /// </summary>
        public float ManaRegenPerSecond => GetScaledStat(
            HeroDefinition != null ? HeroDefinition.BaseManaRegenPerSecond : 0f,
            HeroDefinition != null ? HeroDefinition.ManaRegenPerSecondPerLevel : 0f);

        private void Awake()
        {
            heroRuntime = GetComponent<HeroRuntime>();
            levelSystem = GetComponent<LevelSystem>();
        }

        private void OnEnable()
        {
            if (heroRuntime != null)
            {
                heroRuntime.OnHeroDefinitionChanged += HandleHeroDefinitionChanged;
            }

            if (levelSystem != null)
            {
                levelSystem.OnLevelChanged += HandleLevelChanged;
            }
        }

        private void Start()
        {
            NotifyStatsChanged();
        }

        private void OnDisable()
        {
            if (heroRuntime != null)
            {
                heroRuntime.OnHeroDefinitionChanged -= HandleHeroDefinitionChanged;
            }

            if (levelSystem != null)
            {
                levelSystem.OnLevelChanged -= HandleLevelChanged;
            }
        }

        private void HandleHeroDefinitionChanged(HeroDefinition _)
        {
            NotifyStatsChanged();
        }

        private void HandleLevelChanged(int _)
        {
            NotifyStatsChanged();
        }

        private void NotifyStatsChanged()
        {
            OnStatsChanged?.Invoke();
        }

        private float GetScaledStat(float baseValue, float perLevelValue)
        {
            int levelOffset = Mathf.Max(0, CurrentLevel - 1);
            return baseValue + (perLevelValue * levelOffset);
        }
    }
}