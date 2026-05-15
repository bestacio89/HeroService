using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Progression;
using MobaPrototype.Systems.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Positionne et synchronise l'UI monde attachée à un héros.
    /// </summary>
    public class HeroWorldUI : MonoBehaviour
    {
        [Header("Références systèmes")]
        [SerializeField] private HealthSystem healthSystem;
        [SerializeField] private ManaSystem manaSystem;
        [SerializeField] private LevelSystem levelSystem;

        [Header("HP Bar")]
        [SerializeField] private Image hpFill;
        [SerializeField] private Image hpBackground;

        [Header("Mana Bar")]
        [SerializeField] private Image manaFill;

        [Header("Level Badge")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image levelBadgeImage;

        [Header("Équipe")]
        [SerializeField] private Color allyColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color enemyColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color manaColor = new Color(0.2f, 0.5f, 1f);

        [Header("World Space")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.2f, 0f);
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform followTarget;

        private static readonly Color HpHigh = new Color(0.15f, 0.85f, 0.15f);
        private static readonly Color HpMid = new Color(1f, 0.65f, 0f);
        private static readonly Color HpLow = new Color(0.9f, 0.1f, 0.1f);

        private bool _isAlly = true;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (followTarget == null)
            {
                if (healthSystem != null)
                    followTarget = healthSystem.transform;
                else if (transform.parent != null)
                    followTarget = transform.parent;
            }
        }

        private void OnEnable()
        {
            if (healthSystem != null)
                healthSystem.OnHealthChanged += HandleHealthChanged;
            if (manaSystem != null)
                manaSystem.OnManaChanged += HandleManaChanged;
            if (levelSystem != null)
                levelSystem.OnLevelChanged += HandleLevelChanged;
        }

        private void OnDisable()
        {
            if (healthSystem != null)
                healthSystem.OnHealthChanged -= HandleHealthChanged;
            if (manaSystem != null)
                manaSystem.OnManaChanged -= HandleManaChanged;
            if (levelSystem != null)
                levelSystem.OnLevelChanged -= HandleLevelChanged;
        }

        private void Start()
        {
            RefreshAll();
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (followTarget == null || mainCamera == null)
                return;

            transform.position = followTarget.position + worldOffset;
            transform.forward = mainCamera.transform.forward;
        }

        public void SetTeam(bool isAlly)
        {
            _isAlly = isAlly;

            if (levelBadgeImage != null)
                levelBadgeImage.color = isAlly ? allyColor : enemyColor;

            RefreshAll();
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (hpFill == null) return;

            float ratio = max > 0f ? current / max : 0f;
            hpFill.fillAmount = ratio;

            Color teamTint = _isAlly ? allyColor : enemyColor;

            if (ratio > 0.5f)
                hpFill.color = Color.Lerp(HpMid, teamTint, (ratio - 0.5f) * 2f);
            else
                hpFill.color = Color.Lerp(HpLow, HpMid, ratio * 2f);
        }

        private void HandleManaChanged(float current, float max)
        {
            if (manaFill == null) return;

            manaFill.fillAmount = max > 0f ? current / max : 0f;
            manaFill.color = manaColor;
        }

        private void HandleLevelChanged(int level)
        {
            if (levelText != null)
                levelText.text = level.ToString();
        }

        private void RefreshAll()
        {
            if (healthSystem != null)
                HandleHealthChanged(healthSystem.CurrentHealth, healthSystem.MaxHealth);

            if (manaSystem != null)
                HandleManaChanged(manaSystem.CurrentMana, manaSystem.MaxMana);

            if (levelSystem != null)
                HandleLevelChanged(levelSystem.CurrentLevel);
        }
    }
}