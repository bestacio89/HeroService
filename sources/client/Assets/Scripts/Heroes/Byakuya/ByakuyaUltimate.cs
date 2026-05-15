using System.Collections;
using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Resources;
using MobaPrototype.Systems.Heroes;

namespace MobaPrototype.Heroes.Byakuya
{
    /// <summary>
    /// Gère la compétence ultime de Byakuya.
    /// </summary>
    public class ByakuyaUltimate : MonoBehaviour, IHeroSkill
    {
        [Header("Ultimate Config")]
        [SerializeField] private float radius = 18f;
        [SerializeField] private float duration = 6f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private float damagePerTick = 50f;
        [SerializeField] private float manaCost = 120f;
        [SerializeField] private float cooldown = 95f;
        [SerializeField] private float damageReduction = 0.5f;

        [Header("References")]
        [SerializeField] private ManaSystem manaSystem;
        [SerializeField] private HealthSystem playerHealth;

        [Header("VFX (optionnel)")]
        [SerializeField] private ParticleSystem bankaiGroundVfx;
        [SerializeField] private ByakuyaCombatVFX combatVfx;

        private float _cooldownRemaining;
        private bool _bankaiActive;
        private HeroRuntime _selfRuntime;
        private LayerMask _heroLayerMask;

        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownMax => cooldown;
        public bool IsReady => _cooldownRemaining <= 0f && !_bankaiActive;
        public bool BankaiActive => _bankaiActive;

        private void Awake()
        {
            if (manaSystem == null)
                manaSystem = GetComponent<ManaSystem>();

            if (playerHealth == null)
                playerHealth = GetComponent<HealthSystem>();

            _selfRuntime = GetComponent<HeroRuntime>();
            _heroLayerMask = LayerMask.GetMask("Hero");

            if (_heroLayerMask == 0)
                Debug.LogWarning("[ByakuyaUltimate] Layer 'Hero' introuvable.", this);

            if (_selfRuntime == null)
                Debug.LogWarning("[ByakuyaUltimate] HeroRuntime introuvable.", this);

            if (manaSystem == null)
                Debug.LogWarning("[ByakuyaUltimate] ManaSystem introuvable.", this);

            if (playerHealth == null)
                Debug.LogWarning("[ByakuyaUltimate] HealthSystem introuvable.", this);
        }

        private void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        public void TryCast()
        {
            if (_bankaiActive)
                return;

            if (_cooldownRemaining > 0f)
                return;

            if (manaSystem == null)
            {
                Debug.LogError("[ByakuyaUltimate] ManaSystem NULL.", this);
                return;
            }

            if (!manaSystem.TrySpendMana(manaCost))
                return;

            StartCoroutine(BankaiRoutine());
        }

        public float ApplyDamageReduction(float incomingDamage)
        {
            if (!_bankaiActive)
                return incomingDamage;

            return incomingDamage * (1f - damageReduction);
        }

        private IEnumerator BankaiRoutine()
        {
            _bankaiActive = true;
            _cooldownRemaining = cooldown;

            if (bankaiGroundVfx != null)
            {
                bankaiGroundVfx.transform.position = transform.position;
                bankaiGroundVfx.Play();
            }

            combatVfx?.PlayBankai(transform.position, radius);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                ApplyTickDamage();
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }

            if (bankaiGroundVfx != null)
                bankaiGroundVfx.Stop();

            _bankaiActive = false;
        }

        private void ApplyTickDamage()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, _heroLayerMask);

            foreach (Collider hit in hits)
            {
                HealthSystem health = hit.GetComponentInParent<HealthSystem>();
                if (health == null || health.IsDead)
                    continue;

                HeroRuntime targetRuntime = hit.GetComponentInParent<HeroRuntime>();
                if (targetRuntime == null)
                    continue;

                if (_selfRuntime == null)
                {
                    Debug.LogWarning("[ByakuyaUltimate] Self HeroRuntime NULL pendant ApplyTickDamage().", this);
                    continue;
                }

                if (targetRuntime == _selfRuntime)
                    continue;

                if (targetRuntime.Team == _selfRuntime.Team)
                    continue;

                health.TakeDamage(damagePerTick);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f, 0f, 1f, 0.15f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}