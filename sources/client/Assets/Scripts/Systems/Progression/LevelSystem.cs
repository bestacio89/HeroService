using System;
using UnityEngine;

namespace MobaPrototype.Systems.Progression
{
    /// <summary>
    /// Handles hero level progression from level 1 to max level.
    /// </summary>
    public class LevelSystem : MonoBehaviour
    {
        [Header("Level Rules")]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private int maxLevel = 15;
        [SerializeField] private int baseExperienceToLevelUp = 100;
        [SerializeField] private int experienceGrowthPerLevel = 25;

        [Header("Debug")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentExperience;

        /// <summary>
        /// Raised when the current level changes.
        /// </summary>
        public event Action<int> OnLevelChanged;

        /// <summary>
        /// Raised when the current experience changes.
        /// </summary>
        public event Action<int, int> OnExperienceChanged;

        /// <summary>
        /// Gets the current level.
        /// </summary>
        public int CurrentLevel => currentLevel;

        /// <summary>
        /// Gets the current accumulated experience within the current level.
        /// </summary>
        public int CurrentExperience => currentExperience;

        /// <summary>
        /// Gets the maximum level allowed by this system.
        /// </summary>
        public int MaxLevel => maxLevel;

        private void Awake()
        {
            currentLevel = Mathf.Clamp(startingLevel, 1, maxLevel);
            currentExperience = 0;
        }

        private void Start()
        {
            OnLevelChanged?.Invoke(currentLevel);
            OnExperienceChanged?.Invoke(currentExperience, GetRequiredExperienceForCurrentLevel());
        }

        /// <summary>
        /// Adds experience and levels up as many times as possible.
        /// </summary>
        /// <param name="amount">Amount of experience to add.</param>
        public void AddExperience(int amount)
        {
            if (amount <= 0 || currentLevel >= maxLevel)
            {
                return;
            }

            currentExperience += amount;
            TryProcessLevelUps();
            OnExperienceChanged?.Invoke(currentExperience, GetRequiredExperienceForCurrentLevel());
        }

        /// <summary>
        /// Sets the current level directly for debug or initialization.
        /// </summary>
        /// <param name="newLevel">New level value.</param>
        public void SetLevel(int newLevel)
        {
            int clampedLevel = Mathf.Clamp(newLevel, 1, maxLevel);

            if (currentLevel == clampedLevel)
            {
                return;
            }

            currentLevel = clampedLevel;
            currentExperience = 0;

            OnLevelChanged?.Invoke(currentLevel);
            OnExperienceChanged?.Invoke(currentExperience, GetRequiredExperienceForCurrentLevel());
        }

        /// <summary>
        /// Gets the required experience to go from the current level to the next level.
        /// </summary>
        /// <returns>Required experience for the current level.</returns>
        public int GetRequiredExperienceForCurrentLevel()
        {
            if (currentLevel >= maxLevel)
            {
                return 0;
            }

            return baseExperienceToLevelUp + ((currentLevel - 1) * experienceGrowthPerLevel);
        }

        private void TryProcessLevelUps()
        {
            while (currentLevel < maxLevel)
            {
                int requiredExperience = GetRequiredExperienceForCurrentLevel();

                if (currentExperience < requiredExperience)
                {
                    break;
                }

                currentExperience -= requiredExperience;
                currentLevel++;
                OnLevelChanged?.Invoke(currentLevel);
            }

            if (currentLevel >= maxLevel)
            {
                currentExperience = 0;
            }
        }
    }
}