using System.Collections;
using MobaPrototype.Systems.Health;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Respawns the player after death.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    /// <summary>
    /// Gère le cycle de mort et de respawn du joueur local.
    /// </summary>
    public class PlayerRespawn : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private float respawnDelay = 2f;

        private HealthSystem healthSystem;
        private CharacterController characterController;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            characterController = GetComponent<CharacterController>();
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

        /// <summary>
        /// Appelé par HeroSpawner pour câbler le RespawnPoint depuis la scène.
        /// </summary>
        public void SetRespawnPoint(Transform point)
        {
            respawnPoint = point;
        }

        private void HandleDeath()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (respawnPoint != null)
            {
                if (characterController != null)
                    characterController.enabled = false;

                transform.position = respawnPoint.position;
                transform.rotation = respawnPoint.rotation;

                if (characterController != null)
                    characterController.enabled = true;
            }

            healthSystem.ReviveFull();
        }
    }
}