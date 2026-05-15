using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Resources;
using MobaPrototype.Player;

namespace MobaPrototype.Heroes.Byakuya
{
    /// <summary>
    /// Gère la deuxième compétence active de Byakuya.
    /// </summary>
    public class ByakuyaSkillTwo : MonoBehaviour, IHeroSkill
    {
        [Header("Skill Config")]
        [SerializeField] private float damage = 220f;
        [SerializeField] private float manaCost = 70f;
        [SerializeField] private float cooldown = 12f;
        [SerializeField] private float range = 12f;

        [Header("References")]
        [SerializeField] private ManaSystem manaSystem;
        [SerializeField] private PlayerTargeting playerTargeting;

        [Header("VFX (optionnel)")]
        [SerializeField] private ParticleSystem concentratedBlastVfx;

        private float _cooldownRemaining;

        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownMax => cooldown;
        public bool IsReady => _cooldownRemaining <= 0f;

        private void Awake()
        {
            if (manaSystem == null)
                manaSystem = GetComponent<ManaSystem>();

            if (playerTargeting == null)
                playerTargeting = GetComponent<PlayerTargeting>();

            if (manaSystem == null)
                Debug.LogWarning("[ByakuyaSkillTwo] ManaSystem introuvable.", this);

            if (playerTargeting == null)
                Debug.LogWarning("[ByakuyaSkillTwo] PlayerTargeting introuvable.", this);
        }

        private void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        public void TryCast()
        {
            if (_cooldownRemaining > 0f)
                return;

            if (playerTargeting == null)
            {
                Debug.LogError("[ByakuyaSkillTwo] PlayerTargeting NULL.", this);
                return;
            }

            Transform target = playerTargeting.CurrentTarget;
            if (target == null)
                return;

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > range)
                return;

            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth == null || targetHealth.IsDead)
                return;

            if (manaSystem == null)
            {
                Debug.LogError("[ByakuyaSkillTwo] ManaSystem NULL.", this);
                return;
            }

            if (!manaSystem.TrySpendMana(manaCost))
                return;

            _cooldownRemaining = cooldown;
            targetHealth.TakeDamage(damage);

            if (concentratedBlastVfx != null)
            {
                concentratedBlastVfx.transform.position = transform.position;
                concentratedBlastVfx.transform.forward =
                    (target.position - transform.position).normalized;
                concentratedBlastVfx.Play();
            }
        }
    }
}