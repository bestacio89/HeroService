// Scripts/Systems/Health/StructureHealth.cs
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaPrototype.Systems.Health
{
  /// <summary>
  /// Points de vie d'une STRUCTURE (Core, Merveille, tourelle...).
  ///
  /// Volontairement INDÉPENDANT de HealthSystem :
  /// HealthSystem exige un HeroStatsProvider (donc une HeroDefinition),
  /// ce qui n'a aucun sens pour un bâtiment. On garde donc un composant
  /// simple et autonome, avec ses HP réglés directement dans l'inspecteur.
  ///
  /// Étape A de STEP 7 : la structure n'est pas encore frappée par le
  /// système de combat des héros (ce sera l'étape B, via IDamageable).
  /// Pour tester maintenant, on inflige des dégâts via une touche debug.
  /// </summary>
  public class StructureHealth : MonoBehaviour
  {
    [Header("Vie")]
    [Tooltip("Points de vie maximum de la structure.")]
    [SerializeField] private float maxHealth = 1000f;

    [Header("Debug (test sans combat)")]
    [Tooltip("Active la touche de debug pour infliger des dégâts au clavier.")]
    [SerializeField] private bool enableDebugKey = true;
    [SerializeField] private Key debugDamageKey = Key.K;
    [SerializeField] private float debugDamagePerPress = 250f;

    private float currentHealth;
    private bool isDead;

    /// <summary>Déclenché à chaque changement de vie (current, max). Utile pour une barre de vie.</summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>Déclenché une seule fois, quand la structure atteint 0 PV.</summary>
    public event Action OnDied;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
      currentHealth = maxHealth;
    }

    private void Start()
    {
      // On notifie une première fois pour que l'UI (future barre de vie) s'initialise.
      RaiseHealthChanged();
    }

    private void Update()
    {
      if (!enableDebugKey || isDead)
      {
        return;
      }

      // Nouveau Input System (même convention que PlayerMovement).
      // wasPressedThisFrame = équivalent de l'ancien Input.GetKeyDown.
      Keyboard keyboard = Keyboard.current;
      if (keyboard != null && keyboard[debugDamageKey].wasPressedThisFrame)
      {
        TakeDamage(debugDamagePerPress);
      }
    }

    /// <summary>
    /// Inflige des dégâts à la structure. Sera appelé plus tard par le
    /// système de combat (étape B). Pour l'instant, appelé par la touche debug.
    /// </summary>
    public void TakeDamage(float amount)
    {
      if (isDead || amount <= 0f)
      {
        return;
      }

      currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
      RaiseHealthChanged();

      if (currentHealth <= 0f)
      {
        isDead = true;
        OnDied?.Invoke();
      }
    }

    /// <summary>Détruit instantanément la structure (pour tester depuis l'inspecteur).</summary>
    [ContextMenu("Debug: Détruire la structure")]
    public void DebugKill()
    {
      TakeDamage(maxHealth);
    }

    private void RaiseHealthChanged()
    {
      OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
  }
}
