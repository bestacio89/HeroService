using System.Collections;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Resources;
using UnityEngine;

namespace MobaPrototype.AI
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterController))]
    /// <summary>
    /// Gère le cycle de mort, de temporisation et de respawn des héros NPC.
    /// </summary>
    public class NpcHeroRespawn : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 3f;
        [SerializeField] private float postRespawnFreeze = 0.4f;

        private HealthSystem healthSystem;
        private CharacterController characterController;
        private CapsuleCollider capsuleCollider;
        private NpcHeroAI npcHeroAI;
        private ManaSystem manaSystem;

        private Renderer[] cachedRenderers;
        private Canvas[] cachedCanvases;

        private Vector3 respawnPosition;
        private Quaternion respawnRotation = Quaternion.identity;
        private bool hasRespawnPoint;

        private Coroutine respawnRoutine;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            characterController = GetComponent<CharacterController>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            npcHeroAI = GetComponent<NpcHeroAI>();
            manaSystem = GetComponent<ManaSystem>();

            Transform modelRoot = transform.Find("Model");
            if (modelRoot != null)
                cachedRenderers = modelRoot.GetComponentsInChildren<Renderer>(true);
            else
                cachedRenderers = new Renderer[0];

            Transform heroWorldUiRoot = transform.Find("HeroWorldUI");
            if (heroWorldUiRoot != null)
                cachedCanvases = heroWorldUiRoot.GetComponentsInChildren<Canvas>(true);
            else
                cachedCanvases = new Canvas[0];
        }

        private void OnEnable()
        {
            if (healthSystem != null)
                healthSystem.OnDied += HandleDeath;
        }

        private void OnDisable()
        {
            if (healthSystem != null)
                healthSystem.OnDied -= HandleDeath;
        }

        public void SetRespawnPoint(Transform point)
        {
            if (point == null)
            {
                hasRespawnPoint = false;
                return;
            }

            respawnPosition = point.position;
            respawnRotation = point.rotation;
            hasRespawnPoint = true;
        }

        private void HandleDeath()
        {
            if (!isActiveAndEnabled)
                return;

            if (respawnRoutine != null)
                return;

            respawnRoutine = StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            SetDeadState(true);

            yield return new WaitForSeconds(respawnDelay);

            Vector3 targetPosition = hasRespawnPoint ? respawnPosition : transform.position;
            Quaternion targetRotation = hasRespawnPoint ? respawnRotation : transform.rotation;

            transform.SetPositionAndRotation(targetPosition, targetRotation);

            if (healthSystem != null)
                healthSystem.ReviveFull();

            if (manaSystem != null)
                manaSystem.RestoreFullMana();

            SetVisualState(true);

            // Laisse 1 frame aux systèmes UI/health pour se resynchroniser après ReviveFull
            yield return null;

            if (characterController != null)
                characterController.enabled = true;

            if (capsuleCollider != null)
                capsuleCollider.enabled = true;

            if (postRespawnFreeze > 0f)
                yield return new WaitForSeconds(postRespawnFreeze);

            if (npcHeroAI != null)
                npcHeroAI.enabled = true;

            respawnRoutine = null;
        }

        private void SetDeadState(bool isDead)
        {
            if (npcHeroAI != null)
                npcHeroAI.enabled = !isDead;

            if (characterController != null)
                characterController.enabled = !isDead;

            if (capsuleCollider != null)
                capsuleCollider.enabled = !isDead;

            SetVisualState(!isDead);
        }

        private void SetVisualState(bool visible)
        {
            if (cachedRenderers != null)
            {
                foreach (Renderer renderer in cachedRenderers)
                {
                    if (renderer != null)
                        renderer.enabled = visible;
                }
            }

            if (cachedCanvases != null)
            {
                foreach (Canvas canvas in cachedCanvases)
                {
                    if (canvas != null)
                        canvas.enabled = visible;
                }
            }
        }
    }
}