using System;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerGridController : MonoBehaviour
    {
        [Header("Movement")] public float playerMovementVelocity;
        public float positionReachedTolerance;
        public float rotationSpeed;

        [Header("Stop Same Position")] public float positionStoppedTolerance;
        public int maxStopFrameCount;

        private Rigidbody _playerRb;
        private Collider _playerCollider;

        private PlayerState _playerState;

        // Target Movement
        private Vector3 _targetPosition; // Try reaching as close as possible
        private Vector3 _lastPosition;
        private bool _positionReached;

        // Stop Position
        private int _currentStopFrameCount;

        #region Unity Functions

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();

            _lastPosition = transform.position;
            _positionReached = true;

            SetPlayerState(PlayerState.PlayerInControl);
        }

        private void FixedUpdate()
        {
            switch (_playerState)
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
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TagManager.WinMarker))
            {
                SetPlayerEndState(true);
            }
            else if (other.gameObject.CompareTag(TagManager.WaterHole))
            {
                SetPlayerEndState(false);
            }
            else if (other.gameObject.CompareTag(TagManager.FaceOut))
            {
                transform.SetParent(other.transform.parent);
            }
        }

        #endregion

        #region External Functions

        public void SetPlayerTargetLocation(Vector3 targetPosition)
        {
            if (_playerState != PlayerState.PlayerInControl || IsPlayerMoving())
            {
                return;
            }

            _targetPosition = targetPosition;
            _positionReached = false;
            _currentStopFrameCount = 0;
            _playerRb.useGravity = true;
        }

        public void ResetPlayerGravityState()
        {
            bool isGroundBelow = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f);
            if (!isGroundBelow)
            {
                _playerRb.useGravity = true;
            }
        }

        public bool IsPlayerMoving() => !_positionReached;

        public void PreventPlayerMovement()
        {
            if (_playerState == PlayerState.PlayerEndState)
            {
                return;
            }

            SetPlayerState(PlayerState.PlayerStatic);
            _playerRb.isKinematic = true;
        }

        public void AllowPlayerMovement()
        {
            if (_playerState == PlayerState.PlayerEndState)
            {
                return;
            }

            SetPlayerState(PlayerState.PlayerInControl);
            _playerRb.isKinematic = false;
        }

        #endregion

        #region Utility Functions

        #region Player  Movement

        private void MovePlayer()
        {
            if (_positionReached || _playerState != PlayerState.PlayerInControl)
            {
                return;
            }

            Vector3 movementDirection = _targetPosition - transform.position;
            movementDirection = movementDirection.normalized;

            Vector3 directionVelocity = movementDirection * playerMovementVelocity;
            _playerRb.velocity = new Vector3(
                directionVelocity.x,
                _playerRb.velocity.y,
                directionVelocity.z
            );

            float distance = (transform.position - _targetPosition).magnitude;
            float stopMagDiff = (transform.position - _lastPosition).magnitude;
            if (Mathf.Abs(stopMagDiff) <= positionStoppedTolerance)
            {
                _currentStopFrameCount += 1;
            }

            if (Mathf.Abs(distance) <= positionReachedTolerance || _currentStopFrameCount > maxStopFrameCount)
            {
                _positionReached = true;
                _playerRb.velocity = Vector3.zero;
                _playerRb.useGravity = false;

                Debug.Log("Player Reached Position");
            }

            _lastPosition = transform.position;
        }

        private void OrientPlayerToMovement()
        {
            if (_positionReached)
            {
                return;
            }

            Vector3 currentPosition = transform.position;
            Vector3 direction = _lastPosition - currentPosition;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        #endregion

        private void SetPlayerEndState(bool didPlayerWin)
        {
            // TODO: Complete this function

            Debug.Log($"Player Won: {didPlayerWin}");

            SetPlayerState(PlayerState.PlayerEndState);
            _playerRb.velocity = Vector3.zero;
        }

        private void SetPlayerState(PlayerState playerState) => _playerState = playerState;

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