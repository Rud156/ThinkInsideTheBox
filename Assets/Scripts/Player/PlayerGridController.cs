﻿using Scenes.Main;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerGridController : MonoBehaviour
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
        private SphereCollider _playerCollider;

        // Player State
        private PlayerState _playerState;

        #region Unity Functions

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<SphereCollider>();

            _lastPosition = transform.position;
            _positionReached = true;
            _lerpAmount = 0;

            SetPlayerState(PlayerState.PlayerInControl);
        }

        private void Update()
        {
            switch (_playerState)
            {
                case PlayerState.PlayerInControl:
                {
                    OrientPlayerToPosition();
                    MovePlayer();
                }
                    break;

                case PlayerState.PlayerStatic:
                    // Probably don't do anything here...
                    break;

                case PlayerState.PlayerEndState:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.GridMarker))
            {
                transform.SetParent(other.transform.parent.parent);
            }
            else if (other.CompareTag(TagManager.WaterHole))
            {
                SetPlayerEndState(false);
            }
            else if (other.CompareTag(TagManager.WinMarker))
            {
                SetPlayerEndState(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.GridMarker))
            {
                transform.SetParent(null);
            }
        }

        #endregion

        #region External Functions

        public void SetPlayerTargetLocation(Vector3 targetPosition)
        {
            // Don't detect inputs when the player is not in control
            if (_playerState != PlayerState.PlayerInControl)
            {
                return;
            }

            targetPosition += posiitionOffset;
            _targetPosition = targetPosition;

            _startPosition = transform.position;
            _lerpAmount = 0;
            _positionReached = false;

            _playerRb.isKinematic = true;
            _playerRb.useGravity = false;
            _playerCollider.isTrigger = true;
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
            if (_positionReached)
            {
                return;
            }

            _lastPosition = transform.position;
            _lerpAmount += movementLerpSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, _lerpAmount);

            if (_lerpAmount >= movementLerpTolerance)
            {
                transform.position = _targetPosition;
                _positionReached = true;

                _playerRb.isKinematic = false;
                _playerRb.useGravity = true;
                _playerCollider.isTrigger = false;
            }
        }

        private void OrientPlayerToPosition()
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
            SetPlayerState(PlayerState.PlayerEndState);

            _playerCollider.isTrigger = true;

            if (didPlayerWin)
            {
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                MainSceneController.Instance.LoadNextLevel(buildIndex + 1);
            }
            else
            {
                MainSceneController.Instance.ReloadCurrentLevel();
            }
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