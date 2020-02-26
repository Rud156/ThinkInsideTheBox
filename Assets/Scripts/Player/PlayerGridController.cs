using System;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [Header("Inventory")] public InventorySystem myInventory;

        [Header("Stop Same Position")] public float positionStoppedTolerance;
        public int maxStopFrameCount;

        [Header("RayCast Data")] public float rayCastDistance;
        public LayerMask layerMask;
        public LayerMask gravityCheckLayerMask;

        [Header("Collisions")] public CollisionNotifier collisionNotifier;

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

        // Player Movement Stop and Start
        private bool m_lastPlayerMovementStatus;

        public delegate void WorldFlip(Transform parentTransform);
        public delegate void PlayerMovementLocked();
        public delegate void PlayerMovementUnLocked();
        public delegate void PlayerReachedPosition(bool success);
        public delegate void PlayerStartedMovement();

        public WorldFlip OnWorldFlip;
        public PlayerReachedPosition OnPlayerReachedPosition;
        public PlayerStartedMovement OnPlayerStartedMovement;
        public PlayerMovementLocked OnPlayerMovementLocked;
        public PlayerMovementUnLocked OnPlayerMovementUnLocked;

        #region Unity Functions

        private void Start()
        {
            collisionNotifier.OnTriggerEnterNotifier += HandleTriggerEnter;
            collisionNotifier.OnTriggerExitNotifier += HandleTriggerExit;

            m_playerRb = GetComponent<Rigidbody>();
            m_playerCollider = GetComponent<Collider>();

            m_lastPosition = transform.position;
            m_positionReached = true;

            SetPlayerState(PlayerState.PlayerInControl);
        }

        private void OnDestroy()
        {
            collisionNotifier.OnTriggerEnterNotifier -= HandleTriggerEnter;
            collisionNotifier.OnTriggerExitNotifier -= HandleTriggerExit;
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

            OnPlayerStartedMovement?.Invoke();
        }

        public void ResetPlayerGravityState()
        {
            bool isGroundBelow = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f,
                gravityCheckLayerMask);
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

            if (m_lastPlayerMovementStatus)
            {
                OnPlayerMovementLocked?.Invoke();
                m_lastPlayerMovementStatus = false;
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

            if (!m_lastPlayerMovementStatus)
            {
                OnPlayerMovementUnLocked?.Invoke();
                m_lastPlayerMovementStatus = true;
            }

            SetPlayerState(PlayerState.PlayerInControl);
            m_playerRb.isKinematic = false;
        }

        #endregion

        #region Utility Functions

        #region Collisions

        private void HandleTriggerEnter(Collider i_other)
        {
            if (i_other.CompareTag(TagManager.WinMarker))
            {
                SetPlayerEndState(true);
            }
            else if (i_other.CompareTag(TagManager.WaterHole) && !m_isPlayerOutside)
            {
                CheckAndUpdatePlayerEndingCollision(i_other);
            }
            else if (i_other.transform.parent.CompareTag(TagManager.SideParent))
            {
                transform.SetParent(i_other.transform.parent);
            }
        }

        private void HandleTriggerExit(Collider i_other)
        {
            if (i_other.transform.parent.CompareTag(TagManager.SideParent))
            {
                transform.SetParent(null);
            }
        }

        private void CheckAndUpdatePlayerEndingCollision(Collider i_other)
        {
            Vector3 targetTilePosition = i_other.transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(targetTilePosition, Vector3.down, out RaycastHit hit, 0.2f, layerMask))
            {
                if (hit.collider.CompareTag(i_other.tag))
                {
                    SetPlayerEndState(false);
                }
            }
        }

        #endregion

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
                if (Mathf.Abs(distance) <= positionReachedTolerance)
                {
                    OnPlayerReachedPosition?.Invoke(true);
                }
                else
                {
                    OnPlayerReachedPosition?.Invoke(false);
                }

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
            // TODO: Complete this function. This will change based on more updates to props and other world objects

            Debug.Log($"Did Player Win: {i_didPlayerWin}");
            if (!i_didPlayerWin)
            {
                Debug.Log("Player Died. Reloading the scene");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("Player Won");

                int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentBuildIndex + 1);
            }

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