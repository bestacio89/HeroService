// Scripts/Heroes/Byakuya/ByakuyaCombatVFX.cs
// Binder entre gameplay et VFX pour Byakuya.
// Ne contient aucune logique de gameplay — uniquement des appels visuels.

using UnityEngine;

namespace MobaPrototype.Heroes.Byakuya
{
    /// <summary>
    /// Déclenche les effets visuels liés aux actions de combat de Byakuya.
    /// </summary>
    public class ByakuyaCombatVFX : MonoBehaviour
    {
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem petalCast;
        [SerializeField] private ParticleSystem teleportBurst;
        [SerializeField] private ParticleSystem deathPetals;
        [SerializeField] private ParticleSystem bankaiGroundFx;

        [Header("Materials / Shaders")]
        [SerializeField] private Renderer heroRenderer;
        [SerializeField] private Renderer teleportTrailRenderer;
        [SerializeField] private Renderer bankaiGroundRenderer;

        private static readonly int MoveBlend = Shader.PropertyToID("_MoveBlend");
        private static readonly int HpFade = Shader.PropertyToID("_HpFade");
        private static readonly int ManaGlow = Shader.PropertyToID("_ManaGlow");
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        private static readonly int Radius = Shader.PropertyToID("_Radius");

        /// <summary>
        /// Appelé chaque frame par ByakuyaUltimate et ByakuyaSkillTwo
        /// pour mettre à jour l'état visuel du héros.
        /// </summary>
        public void UpdateHeroState(float hpPercent, float manaPercent, bool isMoving)
        {
            if (heroRenderer == null) return;

            heroRenderer.material.SetFloat(MoveBlend, isMoving ? 1f : 0f);
            heroRenderer.material.SetFloat(HpFade, 1f - hpPercent);
            heroRenderer.material.SetFloat(ManaGlow, manaPercent);
        }

        /// <summary>
        /// Appelé par ByakuyaSkillOne lors du cast AoE pétales.
        /// </summary>
        public void PlayPetalCast(Vector3 origin, Vector3 target)
        {
            if (petalCast == null) return;

            petalCast.transform.position = origin;
            petalCast.transform.forward = (target - origin).normalized;
            petalCast.Play();
        }

        /// <summary>
        /// Appelé par ByakuyaSkillTwo au départ du Shunpo.
        /// </summary>
        public void PlayTeleportStart(Vector3 position)
        {
            if (teleportBurst != null)
            {
                teleportBurst.transform.position = position;
                teleportBurst.Play();
            }

            teleportTrailRenderer?.material.SetFloat(Dissolve, 0f);
        }

        /// <summary>
        /// Appelé par ByakuyaSkillTwo à l'arrivée du Shunpo.
        /// </summary>
        public void PlayTeleportEnd(Vector3 position)
        {
            if (teleportBurst != null)
            {
                teleportBurst.transform.position = position;
                teleportBurst.Play();
            }

            teleportTrailRenderer?.material.SetFloat(Dissolve, 0.35f);
        }

        /// <summary>
        /// Appelé par ByakuyaUltimate au déclenchement du Bankai.
        /// </summary>
        public void PlayBankai(Vector3 center, float radius)
        {
            if (bankaiGroundFx != null)
            {
                bankaiGroundFx.transform.position = center;
                bankaiGroundFx.Play();
            }

            bankaiGroundRenderer?.material.SetFloat(Radius, radius);
        }

        /// <summary>
        /// Appelé à la mort du héros.
        /// </summary>
        public void PlayDeath(Vector3 position)
        {
            if (deathPetals != null)
            {
                deathPetals.transform.position = position;
                deathPetals.Play();
            }

            heroRenderer?.material.SetFloat(Dissolve, 1f);
        }
    }
}