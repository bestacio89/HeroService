using UnityEngine;

namespace MobaPrototype.Systems.Heroes
{
    /// <summary>
    /// Defines the static data of a hero.
    /// This asset contains only design-time data and no runtime state.
    /// </summary>
    [CreateAssetMenu(
        fileName = "HeroDefinition",
        menuName = "MOBA Prototype/Heroes/Hero Definition")]
    /// <summary>
    /// Décrit les données de base d'un héros jouable.
    /// </summary>
    public class HeroDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string heroId = "layla";
        [SerializeField] private string heroDisplayName = "Layla";
        [SerializeField] private HeroClass heroClass = HeroClass.Marksman;

        [Header("Base Stats")]
        [SerializeField] private float baseMaxHealth = 500f;
        [SerializeField] private float baseMaxMana = 300f;
        [SerializeField] private float baseAttackDamage = 50f;
        [SerializeField] private float baseMovementSpeed = 5f;
        [SerializeField] private float baseManaRegenPerSecond = 5f;

        [Header("Per Level Growth")]
        [SerializeField] private float maxHealthPerLevel = 40f;
        [SerializeField] private float maxManaPerLevel = 20f;
        [SerializeField] private float attackDamagePerLevel = 4f;
        [SerializeField] private float movementSpeedPerLevel = 0.05f;
        [SerializeField] private float manaRegenPerSecondPerLevel = 0.2f;

        /// <summary>
        /// Gets the internal hero identifier.
        /// </summary>
        public string HeroId => heroId;

        /// <summary>
        /// Gets the display name shown in UI.
        /// </summary>
        public string HeroDisplayName => heroDisplayName;

        /// <summary>
        /// Gets the hero class.
        /// </summary>
        public HeroClass HeroClassType => heroClass;

        /// <summary>
        /// Gets the base maximum health at level 1.
        /// </summary>
        public float BaseMaxHealth => baseMaxHealth;

        /// <summary>
        /// Gets the base maximum mana at level 1.
        /// </summary>
        public float BaseMaxMana => baseMaxMana;

        /// <summary>
        /// Gets the base attack damage at level 1.
        /// </summary>
        public float BaseAttackDamage => baseAttackDamage;

        /// <summary>
        /// Gets the base movement speed at level 1.
        /// </summary>
        public float BaseMovementSpeed => baseMovementSpeed;

        /// <summary>
        /// Gets the base mana regeneration at level 1.
        /// </summary>
        public float BaseManaRegenPerSecond => baseManaRegenPerSecond;

        /// <summary>
        /// Gets the max health growth applied per level above 1.
        /// </summary>
        public float MaxHealthPerLevel => maxHealthPerLevel;

        /// <summary>
        /// Gets the max mana growth applied per level above 1.
        /// </summary>
        public float MaxManaPerLevel => maxManaPerLevel;

        /// <summary>
        /// Gets the attack damage growth applied per level above 1.
        /// </summary>
        public float AttackDamagePerLevel => attackDamagePerLevel;

        /// <summary>
        /// Gets the movement speed growth applied per level above 1.
        /// </summary>
        public float MovementSpeedPerLevel => movementSpeedPerLevel;

        /// <summary>
        /// Gets the mana regeneration growth applied per level above 1.
        /// </summary>
        public float ManaRegenPerSecondPerLevel => manaRegenPerSecondPerLevel;
    }

    /// <summary>
    /// Supported hero classes for the prototype.
    /// </summary>
    public enum HeroClass
    {
        Marksman,
        Tank,
        Mage,
        Support,
        Fighter
    }
}