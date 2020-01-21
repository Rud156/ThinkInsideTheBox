using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerGridMovement : MonoBehaviour
    {
        [Header("Player Movement")] public float movementLerpSpeed;
        public float movementLerpTolerance;
        public Vector3 posiitionOffset;

        [Header("Player Rotation")] public float rotationSpeed;

        // Movement
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _lerpAmount;
        private bool _positionReached;
        private Vector3 _lastPosition;

        // Components
        private Rigidbody _playerRb;

        // Player State
        private PlayerState _playerState;

        #region Unity Functions

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody>();

            _lastPosition = transform.position;
            _positionReached = true;
            _lerpAmount = 0;

            _playerState = PlayerState.PlayerInControl;
        }

        private void Update()
        {
            switch (_playerState)
            {
                case PlayerState.PlayerInControl:
                {
                    MovePlayer();
                    OrientPlayerToPosition();
                }
                    break;

                case PlayerState.PlayerStatic:
                    // Probably don't do anything here...
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void SetPlayerTargetLocation(Vector3 targetPosition)
        {
            targetPosition += posiitionOffset;
            _targetPosition = targetPosition;

            _startPosition = transform.position;
            _lerpAmount = 0;
            _positionReached = false;
        }

        public void MakePlayerStatic()
        {
            _playerState = PlayerState.PlayerStatic;
            _playerRb.isKinematic = true;
        }

        public void AllowPlayerMovement()
        {
            _playerState = PlayerState.PlayerInControl;
            _playerRb.isKinematic = false;
        }

        #endregion

        #region Utility Functions

        #region Player  Movement

        private void MovePlayer()
        {
            if (_positionReached)
            {
                return;
            }

            _lerpAmount += movementLerpSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, _lerpAmount);

            if (_lerpAmount >= movementLerpTolerance)
            {
                transform.position = _targetPosition;
                _positionReached = true;
            }

            _lastPosition = transform.position;
        }

        private void OrientPlayerToPosition()
        {
            Vector3 currentPosition = transform.position;
            Vector3 direction = _lastPosition - currentPosition;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        #endregion

        #endregion

        #region Enums

        private enum PlayerState
        {
            PlayerInControl,
            PlayerStatic
        }

        #endregion
    }
}