using System;
using Extensions;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerGridController : MonoBehaviour
    {
        [Header("Movement")] public float positionReachedTolerance;
        public float rotationSpeed;
        public float minMovementSpeed;
        public float maxMovementSpeed;

        [Header("Inventory")]public InventorySystem myInventory;

        [Header("Stop Same Position")] public float positionStoppedTolerance;
        public int maxStopFrameCount;

        [Header("RayCast Data")] public float rayCastDistance;
        public LayerMask layerMask;

        private Rigidbody m_playerRb;
        private Collider m_playerCollider;

        private PlayerState m_playerState;

        // Target Movement
        private Vector3 m_targetPosition; // Try reaching as close as possible
        private Vector3 m_lastPosition;
        private float m_maxDistanceToTarget;
        private bool m_positionReached;

        // Stop Position
        private int m_currentStopFrameCount;

        // Boolean to trigger if player is outside
        private bool m_isPlayerOutside;

        public delegate void WorldFlip(Transform parentTransform);

        public WorldFlip OnWorldFlip;

        #region Unity Functions

        private void Start()
        {
            m_playerRb = GetComponent<Rigidbody>();
            m_playerCollider = GetComponent<Collider>();

            m_lastPosition = transform.position;
            m_positionReached = true;

            SetPlayerState(PlayerState.PlayerInControl);
        }

        private void FixedUpdate()
        {
            switch (m_playerState)
            {
                case PlayerState.PlayerInControl:
                    OrientPlayerToMovement();
                    MovePlayer();
                    break;

                case PlayerState.PlayerStatic:
                    break;

                case PlayerState.PlayerEndState:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Use multiple colliders instead
        private void OnCollisionEnter(Collision i_other)
        {
            if (i_other.gameObject.CompareTag(TagManager.WinMarker) && myInventory.Winnable)
            {
                SetPlayerEndState(true);
            }
            else if (i_other.gameObject.CompareTag(TagManager.WaterHole) && !m_isPlayerOutside)
            {
                SetPlayerEndState(false);
            }
            else if (i_other.gameObject.CompareTag(TagManager.FaceOut) ||
                     i_other.gameObject.CompareTag(TagManager.InsideOut))
            {
                transform.SetParent(i_other.transform.parent);
            }
            else if(!myInventory.Winnable)
            {
                Debug.Log("Door Locked");
            }
        }

        #endregion

        #region External Functions

        public void SetPlayerTargetLocation(Vector3 i_targetPosition)
        {
            if (m_playerState != PlayerState.PlayerInControl || IsPlayerMoving())
            {
                return;
            }

            m_targetPosition = i_targetPosition;
            m_maxDistanceToTarget = Vector3.Distance(i_targetPosition, transform.position);
            m_positionReached = false;
            m_currentStopFrameCount = 0;
            m_playerRb.useGravity = true;
        }

        public void ResetPlayerGravityState()
        {
            bool isGroundBelow = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f);
            if (!isGroundBelow)
            {
                m_playerRb.useGravity = true;
            }
        }

        public bool IsPlayerMoving() => !m_positionReached;

        public void PreventPlayerMovement()
        {
            if (m_playerState == PlayerState.PlayerEndState)
            {
                return;
            }

            SetPlayerState(PlayerState.PlayerStatic);
            m_playerRb.isKinematic = true;
        }

        public void AllowPlayerMovement()
        {
            if (m_playerState == PlayerState.PlayerEndState)
            {
                return;
            }

            SetPlayerState(PlayerState.PlayerInControl);
            m_playerRb.isKinematic = false;
        }

        #endregion

        #region Utility Functions

        #region Player  Movement

        private void MovePlayer()
        {
            if (m_positionReached || m_playerState != PlayerState.PlayerInControl)
            {
                return;
            }


            float currentDistance = Vector3.Distance(m_targetPosition, transform.position);
            float mappedSpeed = ExtensionFunctions.Map(
                currentDistance, positionReachedTolerance, m_maxDistanceToTarget,
                maxMovementSpeed, minMovementSpeed
            );

            Vector3 movementDirection = (m_targetPosition - transform.position).normalized;
            Vector3 directionVelocity = movementDirection * mappedSpeed;
            m_playerRb.velocity = new Vector3(
                directionVelocity.x,
                m_playerRb.velocity.y,
                directionVelocity.z
            );

            float distance = (transform.position - m_targetPosition).magnitude;
            float stopMagDiff = (transform.position - m_lastPosition).magnitude;
            if (Mathf.Abs(stopMagDiff) <= positionStoppedTolerance)
            {
                m_currentStopFrameCount += 1;
            }

            if (Mathf.Abs(distance) <= positionReachedTolerance || m_currentStopFrameCount > maxStopFrameCount)
            {
                m_positionReached = true;
                m_playerRb.velocity = Vector3.zero;
                m_playerRb.useGravity = false;

                CheckAndNotifyEndPosition();

                Debug.Log("Player Reached Position");
            }

            m_lastPosition = transform.position;
        }

        private void OrientPlayerToMovement()
        {
            if (m_positionReached)
            {
                return;
            }

            Vector3 currentPosition = transform.position;
            Vector3 direction = m_lastPosition - currentPosition;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        private void CheckAndNotifyEndPosition()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayCastDistance, layerMask))
            {
                if (hit.collider.CompareTag(TagManager.InsideOut))
                {
                    m_isPlayerOutside = !m_isPlayerOutside;
                    OnWorldFlip?.Invoke(hit.collider.transform);
                }
            }
        }

        #endregion

        private void SetPlayerEndState(bool i_didPlayerWin)
        {
            // TODO: Complete this function

            Debug.Log($"Player Won: {i_didPlayerWin}");

            SetPlayerState(PlayerState.PlayerEndState);

            m_playerRb.velocity = Vector3.zero;
            m_playerRb.useGravity = true;
            m_playerRb.isKinematic = false;
            m_positionReached = true;
        }

        private void SetPlayerState(PlayerState i_playerState) => m_playerState = i_playerState;

        #endregion

        #region Enums

        private enum PlayerState
        {
            PlayerInControl,
            PlayerStatic,
            PlayerEndState
        }

        #endregion
    }
}