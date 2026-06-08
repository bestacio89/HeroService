// Scripts/Systems/Health/HealthSystem.cs
using System;
using MobaPrototype.Systems.Heroes;
using MobaPrototype.Managers; // étape B2 : pour TeamId
using UnityEngine;

namespace MobaPrototype.Systems.Health
{
  /// <summary>
  /// Gère les HP d'une entité.
  /// MaxHealth vient de HeroStatsProvider — assigné dynamiquement via HeroRuntime.
  /// Guard : ne déclenche jamais OnDied si MaxHealth = 0 (HeroDefinition pas encore chargée).
  /// </summary>
  [RequireComponent(typeof(HeroStatsProvider))]
  /// <summary>
  /// Gère les points de vie, les dégâts, les soins et les événements de mort.
  /// Étape B2 : implémente IDamageable pour pouvoir être ciblé par le combat
  /// au même titre qu'une structure. L'équipe est lue sur le HeroRuntime du héros.
  /// </summary>
  public class HealthSystem : MonoBehaviour, IDamageable
  {
    [Header("Runtime")]
    [SerializeField] private bool restoreToFullHealthOnStart = true;
    [SerializeField] private float currentHealth;

    private HeroStatsProvider heroStatsProvider;
    private HeroRuntime heroRuntime; // étape B2 : source de l'équipe
    private float cachedMaxHealth;
    private bool isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => heroStatsProvider != null ? heroStatsProvider.MaxHealth : 0f;
    public bool IsDead => isDead;

    // --- IDamageable (étape B2) ---
    /// <summary>Équipe du héros, lue sur son HeroRuntime. TeamA par défaut si absent.</summary>
    public TeamId Team => heroRuntime != null ? heroRuntime.Team : default;
    /// <summary>Transform de l'entité (pour le ciblage).</summary>
    public Transform Transform => transform;

    private void Awake()
    {
      heroStatsProvider = GetComponent<HeroStatsProvider>();
      heroRuntime = GetComponentInParent<HeroRuntime>(); // étape B2
      cachedMaxHealth = MaxHealth;
    }

    private void OnEnable()
    {
      if (heroStatsProvider != null)
        heroStatsProvider.OnStatsChanged += HandleStatsChanged;
    }

    private void OnDisable()
    {
      if (heroStatsProvider != null)
        heroStatsProvider.OnStatsChanged -= HandleStatsChanged;
    }

    private void Start()
    {
      cachedMaxHealth = MaxHealth;

      if (restoreToFullHealthOnStart)
      {
        // ✅ Guard : MaxHealth = 0 signifie HeroDefinition pas encore assignée.
        // On attend HandleStatsChanged() pour initialiser — pas de OnDied.
        if (MaxHealth > 0f)
          RestoreFullHealth();
        else
          currentHealth = 0f;
      }
      else
      {
        // ✅ Ne jamais mourir si MaxHealth = 0 au démarrage
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        isDead = currentHealth <= 0f && MaxHealth > 0f;
        RaiseHealthChanged();
      }
    }

    public void TakeDamage(float amount)
    {
      if (isDead || amount <= 0f) return;

      currentHealth = Mathf.Clamp(currentHealth - amount, 0f, MaxHealth);
      RaiseHealthChanged();

      if (currentHealth <= 0f)
      {
        isDead = true;
        OnDied?.Invoke();
      }
    }

    public void RestoreHealth(float amount)
    {
      if (isDead || amount <= 0f) return;

      float previous = currentHealth;
      currentHealth = Mathf.Clamp(currentHealth + amount, 0f, MaxHealth);

      if (!Mathf.Approximately(previous, currentHealth))
        RaiseHealthChanged();
    }

    public void RestoreFullHealth()
    {
      isDead = false;
      currentHealth = MaxHealth;
      cachedMaxHealth = MaxHealth;
      RaiseHealthChanged();
    }

    public void ReviveFull()
    {
      isDead = false;
      currentHealth = MaxHealth;
      cachedMaxHealth = MaxHealth;
      RaiseHealthChanged();
    }

    private void HandleStatsChanged()
    {
      float oldMax = Mathf.Max(1f, cachedMaxHealth);
      float newMax = Mathf.Max(0f, MaxHealth);

      // Préserve le % de vie lors d'un changement de stats (level up, etc.)
      float ratio = currentHealth / oldMax;
      currentHealth = Mathf.Clamp(newMax * ratio, 0f, newMax);
      cachedMaxHealth = newMax;

      if (newMax <= 0f)
      {
        // ✅ Stats pas encore chargées — silencieux, pas de OnDied
        currentHealth = 0f;
      }
      else if (currentHealth <= 0f)
      {
        // ✅ Mort réelle : on avait des HP, on tombe à 0 après stats change
        isDead = true;
        OnDied?.Invoke();
      }
      else
      {
        isDead = false;
      }

      RaiseHealthChanged();
    }

    private void RaiseHealthChanged()
    {
      OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }
  }
}
