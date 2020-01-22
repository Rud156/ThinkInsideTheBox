using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerGridInput : MonoBehaviour
    {
        [Header("Player")] public Transform playerTransform;
        public PlayerGridMovement playerGridMovement;

        [Header("Raycast")] public Transform leftRaycast;
        public Transform rightRaycast;
        public Transform forwardRaycast;
        public Transform backRaycast;
        public float raycastDistance;

        #region Unity Functions

        private void Update()
        {
            if (Input.GetKeyDown(ControlConstants.Left) || Input.GetKeyDown(ControlConstants.AltLeft))
            {
                FindTargetRayCast(Vector3.right, leftRaycast.position); // TODO: Check why this is opposite
            }
            else if (Input.GetKeyDown(ControlConstants.Right) || Input.GetKeyDown(ControlConstants.AltRight))
            {
                FindTargetRayCast(Vector3.left, rightRaycast.position); // TODO: Check why this is opposite
            }
            else if (Input.GetKeyDown(ControlConstants.Forward) || Input.GetKeyDown(ControlConstants.AltForward))
            {
                FindTargetRayCast(Vector3.forward, forwardRaycast.position);
            }
            else if (Input.GetKeyDown(ControlConstants.Back) || Input.GetKeyDown(ControlConstants.AltBack))
            {
                FindTargetRayCast(Vector3.back, backRaycast.position);
            }

            FollowPlayer();
        }

        #endregion

        #region Utility Functions

        private void FollowPlayer() => transform.position = playerTransform.position;

        private void FindTargetRayCast(Vector3 rayCastDirection, Vector3 position)
        {
            Debug.DrawRay(position, rayCastDirection, Color.red, 3);

            if (Physics.Raycast(position, rayCastDirection, out RaycastHit hit, raycastDistance))
            {
                if (hit.collider.CompareTag(TagManager.GridMarker))
                {
                    Vector3 targetMovementPosition = hit.collider.transform.position;
                    playerGridMovement.SetPlayerTargetLocation(targetMovementPosition);
                }
            }
        }

        #endregion
    }
}