using System;
using Common;
using UnityEngine;
using Utils;
using WorldCube;
using WorldCube.Tile;

namespace Player
{
    [RequireComponent(typeof(PlayerGridController))]
    public class PlayerAutoGridInput : MonoBehaviour
    {
        private const int RotationLocker = 90;

        [Header("Movement")] public float distanceToCheck;
        public float rayCastDownDistance;
        public float collisionCheckDistance;

        [Header("Rotation Tracking")] public Transform rotator;

        [Header("RayCasts")] public float yRayCastOffset;
        public float yRayCastCenterPointOffset;

        [Header("Masks")] public LayerMask layerMask;
        public LayerMask collisionLayerMask;

        [Header("Events")] public CollisionNotifier collisionNotifier;
        public CubeControllerV2 cubeControllerV2;

        [Header("Auto Controls")] public float stopTimeBetweenPositions;
        public Direction startDirection;

        private float m_currentTimer;

        private Vector3 m_lastRotationAngle;
        private AutoMovementState m_autoMovementState;
        private Direction m_currentDirection;

        private PlayerGridController m_playerGridController;

        #region Unity Functions

        private void Start()
        {
            m_playerGridController = GetComponent<PlayerGridController>();

            m_playerGridController.OnPlayerReachedPosition += HandlePlayerPositionReached;
            m_playerGridController.OnPlayerMovementLocked += LockPlayerMovement;
            m_playerGridController.OnPlayerMovementUnLocked += UnLockPlayerMovement;
            m_playerGridController.OnPlayerStartedMovement += HandlePlayerStartMovement;

            collisionNotifier.OnTriggerEnterNotifier += HandleOnTriggerEnter;
            cubeControllerV2.OnWorldClicked += HandleWorldClicked;

            m_currentTimer = stopTimeBetweenPositions;

            SetDirection(startDirection);
            SetPlayerAutoMovementState(AutoMovementState.Waiting);
        }

        private void OnDestroy()
        {
            m_playerGridController.OnPlayerReachedPosition -= HandlePlayerPositionReached;
            m_playerGridController.OnPlayerMovementLocked -= LockPlayerMovement;
            m_playerGridController.OnPlayerMovementUnLocked -= UnLockPlayerMovement;
            m_playerGridController.OnPlayerStartedMovement -= HandlePlayerStartMovement;

            collisionNotifier.OnTriggerEnterNotifier -= HandleOnTriggerEnter;
            cubeControllerV2.OnWorldClicked -= HandleWorldClicked;
        }

        private void Update()
        {
            switch (m_autoMovementState)
            {
                case AutoMovementState.Waiting:
                {
                    if (m_currentTimer > 0)
                    {
                        m_currentTimer -= Time.deltaTime;

                        if (m_currentTimer <= 0)
                        {
                            FindNextMovementSpot();
                        }
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

                case AutoMovementState.ForceStopped:
                    // Player needs to stop. Do nothing
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            HandlePlayerForceStopInput();
        }

        #endregion

        #region Utility Functions

        #region Input

        private void HandlePlayerForceStopInput()
        {
            if (Input.GetKeyDown(ControlConstants.PlayerStopStart))
            {
                if (m_autoMovementState != AutoMovementState.ForceStopped)
                {
                    SetPlayerAutoMovementState(AutoMovementState.ForceStopped);
                }
                else
                {
                    SetPlayerAutoMovementState(AutoMovementState.Waiting);
                }
            }
        }

        #endregion

        #region Collisions

        private void HandleOnTriggerEnter(Collider i_other)
        {
            MovementRotatingTile rotatingTile = i_other.GetComponent<MovementRotatingTile>();
            if (!rotatingTile)
            {
                return;
            }

            Vector3 tileForward = rotatingTile.GetTileForwardDirection();
            Direction direction = GetDirectionFromTileVector(tileForward);

            if (direction != Direction.None)
            {
                SetDirection(direction);
            }
        }

        #endregion

        #region Movement

        private void LockPlayerMovement()
        {
            m_lastRotationAngle = rotator.eulerAngles;

            if (m_autoMovementState == AutoMovementState.ForceStopped)
            {
                return;
            }

            SetPlayerAutoMovementState(AutoMovementState.Stopped);
        }

        private void UnLockPlayerMovement()
        {
            if (m_autoMovementState == AutoMovementState.ForceStopped)
            {
                return;
            }

            SetPlayerAutoMovementState(AutoMovementState.Waiting);
        }

        private void HandlePlayerStartMovement()
        {
        }

        private void FindNextMovementSpot() =>
            FindAndSaveFinalMovementPosition(GetVectorBasedOnDirection(m_currentDirection));

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
                else
                {
                    Debug.Log("Did not find a position. Waiting and then restarting");

                    m_currentTimer = stopTimeBetweenPositions;
                    SetPlayerAutoMovementState(AutoMovementState.Waiting);
                }
            }
        }

        private void HandlePlayerPositionReached(bool success)
        {
            if (m_autoMovementState == AutoMovementState.Stopped)
            {
                return;
            }

            // TODO: This is only a temporary solution. Need to find something better
            if (!success)
            {
                SetDirection(GetOppositeMovementDirection(m_currentDirection));
            }

            m_currentTimer = stopTimeBetweenPositions;
            SetPlayerAutoMovementState(AutoMovementState.Waiting);
        }

        private void HandleWorldClicked()
        {
            Vector3 currentRotationAngle = rotator.eulerAngles;
            Vector3 rotationAngleDiff = m_lastRotationAngle - currentRotationAngle;

            m_lastRotationAngle = currentRotationAngle;
            SetDirection(GetDirectionFromRotation(rotationAngleDiff.y, m_currentDirection));

            Debug.Log($"Forward Difference: {rotationAngleDiff}");

            m_currentTimer = stopTimeBetweenPositions;
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

        #endregion

        #region Positioning/Direction Utilities

        private Direction GetOppositeMovementDirection(Direction i_direction)
        {
            switch (i_direction)
            {
                case Direction.None:
                    throw new ArgumentOutOfRangeException(nameof(i_direction), i_direction, null);

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
                case Direction.None:
                    throw new ArgumentOutOfRangeException(nameof(i_direction), i_direction, null);

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

        private Direction GetDirectionFromTileVector(Vector3 tileForward)
        {
            if (tileForward == Vector3.forward)
            {
                return Direction.Forward;
            }
            else if (tileForward == Vector3.back)
            {
                return Direction.Backward;
            }
            else if (tileForward == Vector3.right)
            {
                return Direction.Left;
            }
            else if (tileForward == Vector3.left)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.None;
            }
        }

        private Direction GetDirectionFromRotation(float i_yRotation, Direction i_currentDirection)
        {
            int normalizedRotation = ExtensionFunctions.GetClosestMultiple(i_yRotation, RotationLocker);
            int totalTurns = Mathf.Abs(normalizedRotation) / RotationLocker;
            int turnDirection = Math.Sign(normalizedRotation);

            for (int i = 0; i < totalTurns; i++)
            {
                switch (i_currentDirection)
                {
                    case Direction.None:
                        break;

                    case Direction.Forward:
                    {
                        if (turnDirection == 1)
                        {
                            i_currentDirection = Direction.Right;
                        }
                        else if (turnDirection == -1)
                        {
                            i_currentDirection = Direction.Left;
                        }
                    }
                        break;

                    case Direction.Backward:
                    {
                        if (turnDirection == 1)
                        {
                            i_currentDirection = Direction.Left;
                        }
                        else if (turnDirection == -1)
                        {
                            i_currentDirection = Direction.Right;
                        }
                    }
                        break;

                    case Direction.Left:
                    {
                        if (turnDirection == 1)
                        {
                            i_currentDirection = Direction.Forward;
                        }
                        else if (turnDirection == -1)
                        {
                            i_currentDirection = Direction.Backward;
                        }
                    }
                        break;

                    case Direction.Right:
                    {
                        if (turnDirection == 1)
                        {
                            i_currentDirection = Direction.Backward;
                        }
                        else if (turnDirection == -1)
                        {
                            i_currentDirection = Direction.Forward;
                        }
                    }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(i_currentDirection), i_currentDirection, null);
                }
            }

            return i_currentDirection;
        }

        #endregion

        private void SetDirection(Direction i_direction) => m_currentDirection = i_direction;

        private void SetPlayerAutoMovementState(AutoMovementState i_autoMovementState) => m_autoMovementState = i_autoMovementState;

        #endregion

        #region Enums

        public enum Direction
        {
            None,

            Forward,
            Backward,
            Left,
            Right
        }

        private enum AutoMovementState
        {
            Waiting,
            Moving,
            Stopped,
            ForceStopped
        }

        #endregion
    }
}