using System;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(PlayerGridController))]
    public class PlayerAutoGridInput : MonoBehaviour
    {
        [Header("Movement")] public float distanceToCheck;
        public float rayCastDownDistance;
        public float collisionCheckDistance;

        [Header("RayCasts")] public float yRayCastOffset;
        public float yRayCastCenterPointOffset;

        [Header("Masks")] public LayerMask layerMask;
        public LayerMask collisionLayerMask;

        [Header("Collisions")] public CollisionNotifier collisionNotifier;

        [Header("Auto Controls")] public float stopTimeBetweenPositions;
        public Direction startDirection;

        private float m_currentTimer;

        private AutoMovementState m_autoMovementState;
        private Direction m_currentDirection;
        private PlayerGridController m_playerGridController;

        #region Unity Functions

        private void Start()
        {
            m_playerGridController.OnPlayerReachedPosition += HandlePlayerPositionReached;
            collisionNotifier.OnTriggerEnterNotifier += HandleOnTriggerEnter;

            m_playerGridController = GetComponent<PlayerGridController>();
            m_currentDirection = startDirection;

            SetPlayerAutoMovementState(AutoMovementState.Waiting);
        }

        private void OnDestroy()
        {
            m_playerGridController.OnPlayerReachedPosition -= HandlePlayerPositionReached;
            collisionNotifier.OnTriggerEnterNotifier -= HandleOnTriggerEnter;
        }

        private void Update()
        {
            switch (m_autoMovementState)
            {
                case AutoMovementState.Waiting:
                {
                    m_currentTimer -= Time.deltaTime;

                    if (m_currentTimer <= 0)
                    {
                        FindNextMovementSpot();
                    }
                }
                    break;

                case AutoMovementState.Moving:
                    // Don't process anything.
                    // PlayerGridController is moving the player
                    break;

                case AutoMovementState.Stopped:
                    // Player needs to stop. Do nothing
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void StartPlayerMovement() => SetPlayerAutoMovementState(AutoMovementState.Waiting);

        public void StopPlayerMovement() => SetPlayerAutoMovementState(AutoMovementState.Stopped);

        #endregion

        #region Utility Functions

        #region Collisions

        private void HandleOnTriggerEnter(Collider i_other)
        {
            if (i_other.CompareTag(TagManager.TurnTile))
            {
                // Get Turn Tile Direction From Orientation
                m_currentDirection = Direction.Forward;
            }
        }

        #endregion

        #region Movement

        private void FindNextMovementSpot() => FindAndSaveFinalMovementPosition(GetVectorBasedOnDirection(m_currentDirection));

        private void FindAndSaveFinalMovementPosition(Vector3 i_moveDirection)
        {
            Vector3 targetMovePosition = GetTargetMovePosition(i_moveDirection);
            if (targetMovePosition != Vector3.one)
            {
                Debug.Log($"Sending Player Position: {targetMovePosition}");

                m_playerGridController.SetPlayerTargetLocation(targetMovePosition);
                SetPlayerAutoMovementState(AutoMovementState.Moving);
            }
            else
            {
                SetDirection(GetOppositeMovementDirection(m_currentDirection));
                Vector3 moveDirection = GetVectorBasedOnDirection(m_currentDirection);

                Vector3 newTargetMovementPosition = GetTargetMovePosition(moveDirection);
                if (newTargetMovementPosition != Vector3.one)
                {
                    Debug.Log($"Sending Player Opposite Position: {newTargetMovementPosition}");

                    m_playerGridController.SetPlayerTargetLocation(newTargetMovementPosition);
                    SetPlayerAutoMovementState(AutoMovementState.Moving);
                }
            }
        }

        private void HandlePlayerPositionReached()
        {
            if (m_autoMovementState == AutoMovementState.Stopped)
            {
                return;
            }

            m_currentTimer = stopTimeBetweenPositions;
            SetPlayerAutoMovementState(AutoMovementState.Waiting);
        }

        #endregion

        #region Positioning

        private Vector3 GetTargetMovePosition(Vector3 i_direction)
        {
            Vector3 position = transform.position + Vector3.up * yRayCastOffset;

            Debug.DrawRay(position, i_direction * collisionCheckDistance, Color.red, 3);
            if (Physics.Raycast(position, i_direction, out RaycastHit collisionHit,
                collisionCheckDistance, collisionLayerMask)) // This is the case that there is an obstacle in the way
            {
                Debug.Log("Invalid Position Collision");
                return Vector3.one;
            }

            Vector3 upperTargetPosition =
                transform.position + i_direction * distanceToCheck + Vector3.up * yRayCastOffset;

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

        private Vector3 FindPositionOnFace(Vector3 objectCenter)
        {
            Vector3 targetPosition = objectCenter + Vector3.up * yRayCastCenterPointOffset;

            Debug.DrawRay(targetPosition, Vector3.down * rayCastDownDistance, Color.green, 3);
            if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, rayCastDownDistance, layerMask))
            {
                return hit.point;
            }

            return Vector3.one;
        }

        private Direction GetOppositeMovementDirection(Direction i_direction)
        {
            switch (i_direction)
            {
                case Direction.Forward:
                    return Direction.Backward;

                case Direction.Backward:
                    return Direction.Forward;

                case Direction.Left:
                    return Direction.Right;

                case Direction.Right:
                    return Direction.Left;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_direction), i_direction, null);
            }
        }

        private Vector3 GetVectorBasedOnDirection(Direction i_direction)
        {
            switch (i_direction)
            {
                case Direction.Forward:
                    return Vector3.forward;

                case Direction.Backward:
                    return Vector3.back;

                case Direction.Left:
                    return Vector3.right;

                case Direction.Right:
                    return Vector3.left;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_direction), i_direction, null);
            }
        }

        #endregion

        private void SetDirection(Direction i_direction) => m_currentDirection = i_direction;

        private void SetPlayerAutoMovementState(AutoMovementState i_autoMovementState) => m_autoMovementState = i_autoMovementState;

        #endregion

        #region Enums

        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right
        }

        private enum AutoMovementState
        {
            Waiting,
            Moving,
            Stopped
        }

        #endregion
    }
}