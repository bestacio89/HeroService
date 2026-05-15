using System.Collections;
using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Resources;
using MobaPrototype.Systems.Heroes;

namespace MobaPrototype.Heroes.Byakuya
{
    /// <summary>
    /// Gère la première compétence active de Byakuya.
    /// </summary>
    public class ByakuyaSkillOne : MonoBehaviour, IHeroSkill
    {
        [Header("Skill Config")]
        [SerializeField] private float radius = 4f;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private float damagePerTick = 40f;
        [SerializeField] private float manaCost = 55f;
        [SerializeField] private float cooldown = 9f;

        [Header("VFX (optionnel)")]
        [SerializeField] private ParticleSystem petalAoeVfx;

        [Header("References")]
        [SerializeField] private ManaSystem manaSystem;

        private float _cooldownRemaining;
        private bool _isActive;
        private HeroRuntime _selfRuntime;
        private LayerMask _heroLayerMask;

        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownMax => cooldown;
        public bool IsReady => _cooldownRemaining <= 0f && !_isActive;

        private void Awake()
        {
            if (manaSystem == null)
                manaSystem = GetComponent<ManaSystem>();

            _selfRuntime = GetComponent<HeroRuntime>();
            _heroLayerMask = LayerMask.GetMask("Hero");

            if (_heroLayerMask == 0)
                Debug.LogWarning("[ByakuyaSkillOne] Layer 'Hero' introuvable.", this);

            if (_selfRuntime == null)
                Debug.LogWarning("[ByakuyaSkillOne] HeroRuntime introuvable.", this);

            if (manaSystem == null)
                Debug.LogWarning("[ByakuyaSkillOne] ManaSystem introuvable.", this);
        }

        private void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        public void TryCast()
        {
            if (_isActive)
                return;

            if (_cooldownRemaining > 0f)
                return;

            if (manaSystem == null)
            {
                Debug.LogError("[ByakuyaSkillOne] ManaSystem NULL.", this);
                return;
            }

            if (!manaSystem.TrySpendMana(manaCost))
                return;

            StartCoroutine(SkillOneRoutine());
        }

        private IEnumerator SkillOneRoutine()
        {
            _isActive = true;
            _cooldownRemaining = cooldown;

            if (petalAoeVfx != null)
                petalAoeVfx.Play();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                ApplyTickDamage();
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }

            if (petalAoeVfx != null)
                petalAoeVfx.Stop();

            _isActive = false;
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
                    Debug.LogWarning("[ByakuyaSkillOne] Self HeroRuntime NULL pendant ApplyTickDamage().", this);
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
            Gizmos.color = new Color(0.6f, 0.2f, 1f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}