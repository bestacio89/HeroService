// Scripts/Systems/Health/StructureHealth.cs
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MobaPrototype.Managers; // étape B2 : pour TeamId

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
  /// Étape B2 : la structure implémente désormais IDamageable. Elle peut
  /// donc être ciblée et frappée par le système de combat des héros, au
  /// même titre qu'un héros — sans pour autant être un héros. Son équipe
  /// est réglée dans l'inspecteur (un Core ennemi = équipe adverse au joueur).
  /// La touche debug reste disponible pour tester sans combat.
  /// </summary>
  public class StructureHealth : MonoBehaviour, IDamageable
  {
    [Header("Équipe")]
    [Tooltip("Équipe de la structure. Un Core ennemi doit être dans l'équipe adverse au joueur (TeamB si le joueur est TeamA).")]
    [SerializeField] private TeamId team = TeamId.TeamB;

    [Header("Vie")]
    [Tooltip("Points de vie maximum de la structure.")]
    [SerializeField] private float maxHealth = 1000f;

    [Header("Debug (test sans combat)")]
    [Tooltip("Active la touche de debug pour infliger des dégâts au clavier.")]
    [SerializeField] private bool enableDebugKey = true;
    [SerializeField] private Key debugDamageKey = Key.K;
    [SerializeField] private float debugDamagePerPress = 250f;

    // DIAGNOSTIC TEMPORAIRE (B4) : affiche dans la Console chaque coup reçu.
    // À repasser sur false (ou à retirer) une fois le combat validé.
    [Tooltip("Diagnostic B4 : logge chaque dégât reçu dans la Console.")]
    [SerializeField] private bool logDamageForDebug = true;

    private float currentHealth;
    private bool isDead;

    /// <summary>Déclenché à chaque changement de vie (current, max). Utile pour une barre de vie.</summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>Déclenché une seule fois, quand la structure atteint 0 PV.</summary>
    public event Action OnDied;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    // --- IDamageable (étape B2) ---
    /// <summary>Équipe de la structure (réglée dans l'inspecteur).</summary>
    public TeamId Team => team;
    /// <summary>Transform de la structure (pour le ciblage).</summary>
    public Transform Transform => transform;

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
    /// Inflige des dégâts à la structure. Appelé par le système de combat
    /// (via IDamageable) ou, pour tester, par la touche debug.
    /// </summary>
    public void TakeDamage(float amount)
    {
      if (isDead || amount <= 0f)
      {
        return;
      }

      currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
      RaiseHealthChanged();

      // DIAGNOSTIC TEMPORAIRE (B4) : prouve que les dégâts arrivent et d'où.
      if (logDamageForDebug)
      {
        Debug.Log($"[StructureHealth] {name} a reçu {amount} dégâts — PV : {currentHealth}/{maxHealth}", this);
      }

      if (currentHealth <= 0f)
      {
        isDead = true;

        if (logDamageForDebug)
        {
          Debug.Log($"[StructureHealth] {name} DÉTRUIT.", this);
        }

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
