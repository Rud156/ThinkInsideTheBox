﻿using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerGridInput : MonoBehaviour
    {
        [Header("Movement")] public float distanceToCheck;
        public float rayCastDownDistance;
        public float collisionCheckDistance;

        [Header("RayCasts")] public float yRayCastOffset;
        public float yRayCastCenterPointOffset;

        [Header("Masks")] public LayerMask layerMask;
        public LayerMask collisionLayerMask;

        [Header("Player Movement")] public Transform playerTransform;
        public PlayerGridController playerGridController;

        #region Unity Functions

        private void Update()
        {
            if (Input.GetKeyDown(ControlConstants.Left) || Input.GetKeyDown(ControlConstants.AltLeft))
            {
                FindAndSaveFinalMovementPosition(Vector3.right);
            }
            else if (Input.GetKeyDown(ControlConstants.Right) || Input.GetKeyDown(ControlConstants.AltRight))
            {
                FindAndSaveFinalMovementPosition(Vector3.left);
            }
            else if (Input.GetKeyDown(ControlConstants.Forward) || Input.GetKeyDown(ControlConstants.AltForward))
            {
                FindAndSaveFinalMovementPosition(Vector3.forward);
            }
            else if (Input.GetKeyDown(ControlConstants.Back) || Input.GetKeyDown(ControlConstants.AltBack))
            {
                FindAndSaveFinalMovementPosition(Vector3.back);
            }
        }

        #endregion

        #region Utility Function

        private void FindAndSaveFinalMovementPosition(Vector3 i_moveDirection)
        {
            Vector3 targetMovePosition = GetTargetMovePosition(i_moveDirection);
            if (targetMovePosition != Vector3.one)
            {
                Debug.Log($"Sending Player Position: {targetMovePosition}");
                playerGridController.SetPlayerTargetLocation(targetMovePosition);
            }
        }

        private Vector3 GetTargetMovePosition(Vector3 i_direction)
        {
            // This might probably not be required
            Vector3 position = playerTransform.position + Vector3.up * yRayCastOffset;

            Debug.DrawRay(position, i_direction * collisionCheckDistance, Color.red, 3);
            if (Physics.Raycast(position, i_direction, out RaycastHit collisionHit,
                collisionCheckDistance, collisionLayerMask)) // This is the case that there is an obstacle in the way
            {
                Debug.Log("Invalid Position Collision");
                return Vector3.one;
            }

            Vector3 upperTargetPosition =
                playerTransform.position + i_direction * distanceToCheck + Vector3.up * yRayCastOffset;

            Debug.DrawRay(upperTargetPosition, Vector3.down * rayCastDownDistance, Color.blue, 3);
            if (Physics.Raycast(upperTargetPosition, Vector3.down, out RaycastHit hit, rayCastDownDistance, layerMask))
            {
                Debug.Log("Valid Position Found");

                Vector3 objectCenter = hit.collider.transform.position; // Get the center of the object hit
                return FindPositionOnFace(objectCenter);
            }

            Debug.Log("Nothing Found");
            return Vector3.one; // This must be ignored
        }

        public Vector3 FindPositionOnFace(Vector3 objectCenter)
        {
            Vector3 targetPosition = objectCenter + Vector3.up * yRayCastCenterPointOffset;

            Debug.DrawRay(targetPosition, Vector3.down * rayCastDownDistance, Color.green, 3);
            if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, rayCastDownDistance, layerMask))
            {
                return hit.point;
            }

            return Vector3.one;
        }
    }

    #endregion
}