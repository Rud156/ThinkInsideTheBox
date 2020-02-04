using System;
using Scenes.Main;
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

        // Flipping related
        private Transform _originalParent;
        private Transform _flipParent;
        private Quaternion _targetRotation;
        private Quaternion _cubeRotation;
        [SerializeField] private Transform _CubeWorld;
        private bool bRotateCube;

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

                int layerMask = 1 << 9;
                // Probably do this somewhere else. Should be a better way to do it.
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayCastDistance, layerMask))
                {
                    Debug.Log(hit.collider);
                    
                    if (hit.collider.CompareTag(TagManager.WaterHole))
                    {
                        SetPlayerEndState(false);
                    }
                    else if(hit.collider.CompareTag(TagManager.InsideOut))
                    {
                        //hit.collider.transform.parent.transform.rotation = Quaternion.Lerp()
                        Transform parent = hit.collider.transform;
                        //Debug.Log(hit.collider + ", " + hit.collider);
                        Vector3 targetEuler = parent.transform.eulerAngles * -1;
                        _targetRotation = Quaternion.Euler(targetEuler );

                        Vector3 cubeTargetRotation = _CubeWorld.eulerAngles + new Vector3(-180, 0, 0);
                        _cubeRotation = Quaternion.Euler(cubeTargetRotation);

                        _originalParent = this.transform.parent;
                        _flipParent = parent;
                        this.transform.parent = (hit.collider.transform);
                        Debug.Log(_originalParent +", " + _flipParent);
                        _playerRb.isKinematic = true;
                        _playerRb.useGravity = false;
                        //SetPlayerEndState(false);
                        Debug.Log("Current player parent: " + this.transform.parent);
                        bRotateCube = false;
                    }
                }
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

        private void FlipCubeFacet()
        {
            if(_originalParent && _flipParent && !bRotateCube)
            {
                this.transform.parent = _flipParent;
                //this.transform.SetParent(_flipParent);
                _flipParent.rotation = Quaternion.Lerp(_flipParent.rotation, _targetRotation, rotationSpeed * Time.deltaTime);

                //Debug.Log(_flipParent.rotation.eulerAngles +", "+ _targetRotation.eulerAngles);
                if(_flipParent.rotation.eulerAngles == _targetRotation.eulerAngles)
                {
                    //Debug.Log("Flip finished");
                    
                    //this.transform.parent = _originalParent;
                    bRotateCube = true;
                }
            }
            if(bRotateCube)
            {
                _CubeWorld.rotation = Quaternion.Lerp(_CubeWorld.rotation, _cubeRotation, rotationSpeed * Time.deltaTime);
                if(_CubeWorld.rotation.eulerAngles == _cubeRotation.eulerAngles)
                {
                    //this.transform.position = _flipParent.position + new Vector3(0, 10, 0);
                    this.transform.parent = _originalParent;
                    _flipParent = null;
                    _originalParent = null;
                    bRotateCube = false;
                    _playerRb.isKinematic = false;
                    _playerRb.useGravity = true;
                }
            }
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