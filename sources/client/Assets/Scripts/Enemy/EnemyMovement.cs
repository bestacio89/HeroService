using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Handles enemy movement toward a target position.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    /// <summary>
    /// Gère les déplacements de l'ancien système d'ennemis.
    /// </summary>
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController characterController;
        private Vector3 verticalVelocity;
        private bool canMove = true;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Moves the enemy toward the given world position.
        /// </summary>
        public void MoveTowards(Vector3 targetPosition)
        {
            if (!canMove || characterController == null)
                return;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Vector3 moveDirection = direction.normalized;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }

            ApplyGravity();
        }

        /// <summary>
        /// Stops horizontal movement but still applies gravity.
        /// </summary>
        public void Stop()
        {
            ApplyGravity();
        }

        public void SetCanMove(bool value)
        {
            canMove = value;
        }

        private void ApplyGravity()
        {
            if (characterController == null || !characterController.enabled)
                return;

            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            characterController.Move(verticalVelocity * Time.deltaTime);
        }
    }
}