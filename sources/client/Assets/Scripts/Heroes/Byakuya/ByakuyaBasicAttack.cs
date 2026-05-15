using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Systems.Health;
using MobaPrototype.Player;

namespace MobaPrototype.Heroes.Byakuya
{
    /// <summary>
    /// Exécute l'attaque de base du héros Byakuya.
    /// </summary>
    public class ByakuyaBasicAttack : MonoBehaviour, IHeroSkill
    {
        [Header("Attack Config")]
        [SerializeField] private float baseDamage = 121f;
        [SerializeField] private float petalBonus = 18f;
        [SerializeField] private float range = 3.2f;
        [SerializeField] private float cooldown = 0.97f;

        [Header("References")]
        [SerializeField] private PlayerTargeting playerTargeting;
        [SerializeField] private ByakuyaSkillOne skillOne;

        [Header("VFX (optionnel)")]
        [SerializeField] private ParticleSystem swordStrikeVfx;
        [SerializeField] private ParticleSystem petalStrikeVfx;

        private float _cooldownRemaining;

        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownMax => cooldown;
        public bool IsReady => _cooldownRemaining <= 0f;

        private void Awake()
        {
            if (playerTargeting == null)
                playerTargeting = GetComponent<PlayerTargeting>();

            if (skillOne == null)
                skillOne = GetComponent<ByakuyaSkillOne>();

            if (playerTargeting == null)
                Debug.LogError("[ByakuyaBasicAttack] PlayerTargeting introuvable.", this);
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
                Debug.LogError("[ByakuyaBasicAttack] PlayerTargeting NULL.", this);
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

            _cooldownRemaining = cooldown;

            float damage = baseDamage;
            bool shikaiActive = skillOne != null && !skillOne.IsReady;

            if (shikaiActive)
            {
                damage += petalBonus;
                petalStrikeVfx?.Play();
            }
            else
            {
                swordStrikeVfx?.Play();
            }

            targetHealth.TakeDamage(damage);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}