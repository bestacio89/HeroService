using MobaPrototype.Managers;
using MobaPrototype.Player;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Progression;
using MobaPrototype.Systems.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Presenter du HUD joueur local pendant le gameplay (barre HP, mana, niveau, cooldowns).
    ///
    /// Responsabilités :
    /// - résoudre les composants du héros local via GameManager au démarrage du match,
    /// - afficher la barre de HP, la barre de mana et le niveau,
    /// - afficher les overlays de cooldown + timer numérique des 4 skill slots,
    /// - se désabonner proprement à la destruction.
    ///
    /// Contraintes d'architecture :
    /// - Aucune logique gameplay dans cette classe.
    /// - Le héros local est résolu via GameManager — jamais via FindGameObjectWithTag.
    /// - Les skill slots sont résolus par nom sur les enfants du héros.
    /// - Distinct de HeroWorldUI (world space) : ce presenter est en screen space, bas d'écran.
    /// </summary>
    public class LocalHeroHUDPresenter : MonoBehaviour
    {
        [Header("Références managers")]
        [SerializeField] private GameManager gameManager;

        [Header("HP")]
        [SerializeField] private Image hpFillImage;
        [SerializeField] private TMP_Text hpText;

        [Header("Mana")]
        [SerializeField] private Image manaFillImage;
        [SerializeField] private TMP_Text manaText;

        [Header("Niveau")]
        [SerializeField] private TMP_Text levelText;

        [Header("Cooldown Overlays — Image radiale par skill")]
        [SerializeField] private Image overlayBasic;
        [SerializeField] private Image overlaySkillOne;
        [SerializeField] private Image overlaySkillTwo;
        [SerializeField] private Image overlayUltimate;

        [Header("Cooldown Timers — Texte numérique par skill")]
        [SerializeField] private TMP_Text timerBasic;
        [SerializeField] private TMP_Text timerSkillOne;
        [SerializeField] private TMP_Text timerSkillTwo;
        [SerializeField] private TMP_Text timerUltimate;

        // Noms des GameObjects enfants des slots dans le Hero_Prefab
        private const string SlotBasicName = "SlotBasic";
        private const string SlotSkillOneName = "SlotSkillOne";
        private const string SlotSkillTwoName = "SlotSkillTwo";
        private const string SlotUltimateName = "SlotUltimate";

        // Composants résolus dynamiquement sur le héros local
        private HealthSystem _healthSystem;
        private ManaSystem _manaSystem;
        private LevelSystem _levelSystem;
        private HeroSkillSlot _slotBasic;
        private HeroSkillSlot _slotSkillOne;
        private HeroSkillSlot _slotSkillTwo;
        private HeroSkillSlot _slotUltimate;

        private bool _isInitialized;

        // ---------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------

        private void Awake()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;

            if (gameManager != null)
                gameManager.OnMatchStarted += HandleMatchStarted;
        }

        private void OnDisable()
        {
            if (gameManager != null)
                gameManager.OnMatchStarted -= HandleMatchStarted;

            UnsubscribeFromHeroSystems();
        }

        private void Update()
        {
            if (!_isInitialized) return;
            RefreshCooldowns();
        }

        // ---------------------------------------------------------------
        // Initialisation
        // ---------------------------------------------------------------

        /// <summary>
        /// Déclenché quand GameManager.StartMatch() est appelé.
        /// C'est le bon moment pour résoudre le héros local — il est garanti présent.
        /// </summary>
        private void HandleMatchStarted()
        {
            if (gameManager == null || gameManager.LocalHero == null)
            {
                Debug.LogWarning("[LocalHeroHUD] LocalHero introuvable au démarrage du match.");
                return;
            }

            ResolveAndSubscribe(gameManager.LocalHero);
        }

        private void ResolveAndSubscribe(GameObject hero)
        {
            UnsubscribeFromHeroSystems();

            // Systèmes — directement sur la racine du héros
            _healthSystem = hero.GetComponent<HealthSystem>();
            _manaSystem = hero.GetComponent<ManaSystem>();
            _levelSystem = hero.GetComponent<LevelSystem>();

            // Slots — sur les GameObjects enfants, résolus par nom
            _slotBasic = ResolveSlot(hero, SlotBasicName);
            _slotSkillOne = ResolveSlot(hero, SlotSkillOneName);
            _slotSkillTwo = ResolveSlot(hero, SlotSkillTwoName);
            _slotUltimate = ResolveSlot(hero, SlotUltimateName);

            if (_healthSystem == null)
                Debug.LogWarning("[LocalHeroHUD] HealthSystem introuvable sur le héros local.");
            if (_manaSystem == null)
                Debug.LogWarning("[LocalHeroHUD] ManaSystem introuvable sur le héros local.");
            if (_levelSystem == null)
                Debug.LogWarning("[LocalHeroHUD] LevelSystem introuvable sur le héros local.");

            // Abonnements events
            if (_healthSystem != null)
                _healthSystem.OnHealthChanged += HandleHealthChanged;
            if (_manaSystem != null)
                _manaSystem.OnManaChanged += HandleManaChanged;
            if (_levelSystem != null)
                _levelSystem.OnLevelChanged += HandleLevelChanged;

            _isInitialized = true;
            RefreshAll();
        }

        /// <summary>
        /// Cherche un HeroSkillSlot sur un enfant du héros par nom de GameObject.
        /// Log un warning si introuvable — non bloquant.
        /// </summary>
        private HeroSkillSlot ResolveSlot(GameObject hero, string childName)
        {
            Transform child = hero.transform.Find(childName);

            if (child == null)
            {
                Debug.LogWarning($"[LocalHeroHUD] Enfant '{childName}' introuvable sur {hero.name}.");
                return null;
            }

            HeroSkillSlot slot = child.GetComponent<HeroSkillSlot>();

            if (slot == null)
                Debug.LogWarning($"[LocalHeroHUD] HeroSkillSlot manquant sur '{childName}'.");

            return slot;
        }

        private void UnsubscribeFromHeroSystems()
        {
            if (_healthSystem != null)
                _healthSystem.OnHealthChanged -= HandleHealthChanged;
            if (_manaSystem != null)
                _manaSystem.OnManaChanged -= HandleManaChanged;
            if (_levelSystem != null)
                _levelSystem.OnLevelChanged -= HandleLevelChanged;

            _isInitialized = false;
        }

        // ---------------------------------------------------------------
        // Handlers événements
        // ---------------------------------------------------------------

        private void HandleHealthChanged(float current, float max) => RefreshHP(current, max);
        private void HandleManaChanged(float current, float max) => RefreshMana(current, max);
        private void HandleLevelChanged(int level) => RefreshLevel(level);

        // ---------------------------------------------------------------
        // Refresh affichage
        // ---------------------------------------------------------------

        private void RefreshAll()
        {
            if (_healthSystem != null)
                RefreshHP(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
            if (_manaSystem != null)
                RefreshMana(_manaSystem.CurrentMana, _manaSystem.MaxMana);
            if (_levelSystem != null)
                RefreshLevel(_levelSystem.CurrentLevel);

            RefreshCooldowns();
        }

        private void RefreshHP(float current, float max)
        {
            if (hpFillImage != null && max > 0f)
                hpFillImage.fillAmount = current / max;

            if (hpText != null)
                hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        private void RefreshMana(float current, float max)
        {
            if (manaFillImage != null && max > 0f)
                manaFillImage.fillAmount = current / max;

            if (manaText != null)
                manaText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        private void RefreshLevel(int level)
        {
            if (levelText != null)
                levelText.text = level.ToString();
        }

        /// <summary>
        /// Met à jour les overlays et timers de cooldown en temps réel.
        /// L'overlay se remplit proportionnellement au cooldown restant.
        /// Le timer affiche les secondes restantes et se cache quand le skill est prêt.
        /// </summary>
        private void RefreshCooldowns()
        {
            RefreshCooldownSlot(overlayBasic, timerBasic, _slotBasic);
            RefreshCooldownSlot(overlaySkillOne, timerSkillOne, _slotSkillOne);
            RefreshCooldownSlot(overlaySkillTwo, timerSkillTwo, _slotSkillTwo);
            RefreshCooldownSlot(overlayUltimate, timerUltimate, _slotUltimate);
        }

        private static void RefreshCooldownSlot(Image overlay, TMP_Text timer, HeroSkillSlot slot)
        {
            bool onCooldown = slot != null && slot.IsBound && slot.CooldownRemaining > 0f;

            // Overlay
            if (overlay != null)
            {
                float max = slot != null ? slot.CooldownMax : 0f;
                overlay.fillAmount = onCooldown && max > 0f
                    ? slot.CooldownRemaining / max
                    : 0f;
            }

            // Timer numérique — visible seulement pendant le cooldown
            if (timer != null)
            {
                if (onCooldown)
                {
                    // Affiche 1 décimale si < 3 secondes, entier sinon — comme Mobile Legends
                    timer.text = Mathf.CeilToInt(slot.CooldownRemaining).ToString();
                    timer.gameObject.SetActive(true);
                }
                else
                {
                    timer.gameObject.SetActive(false);
                }
            }
        }
    }
}
