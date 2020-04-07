using System;
using System.Collections.Generic;
using Audio;
using CustomCamera;
using Player;
using UnityEngine;
using Utils;
using CubeData;

namespace WorldCube
{
    public class CubeControllerV2 : MonoBehaviour
    {
        [Header("Position Data")] public int tileDistance;
        public int rotationDelta;

        [Header("Layers")] public List<CubeLayerObjectV2> cubeLayers;

        [Header("World Flip")] public Transform worldRoot;
        public float lerpSpeed;
        public float lerpEndingAmount = 0.97f;

        [Header("Markers")] public List<GameObject> centerBlocks;
        public CubeLayerPlayerFollower layerPlayerFollower;

        public CameraController cameraController;
        //
        // [Header("World Data")]
        // public GameObject outsideWorld; // TODO: Check how narrative works with this. What other changes are required?

        [Header("Audio")] public AudioController audioController;

        public delegate void WorldClicked();

        public WorldClicked OnWorldClicked;

        private PlayerGridController m_playerGridController;
        private CubeLayerMaskV2 m_lastLayerMask;

        // World Flip
        private Transform m_playerPreviousParent;
        private Transform m_flipTarget;
        private Vector3 m_startRotation;
        private Vector3 m_targetRotation;
        private float m_lerpAmount;
        private bool m_isPlayerOutside;

        private bool m_isMovementAllowed;
        private WorldState m_worldState;

        #region Unity Functions

        private void Start()
        {
            //m_playerGridController = GameObject.FindGameObjectWithTag(TagManager.Player)
            //    .GetComponent<PlayerGridController>();
            //m_playerGridController.OnWorldFlip += InitiateWorldFlip;

            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                cubeLayerObjectV2.SetTileDistance(tileDistance);
            }

            SetupInitialWorldState();
            SetWorldState(WorldState.ControllerControlled);
        }

        private void OnDestroy()
        {
            //m_playerGridController.OnWorldFlip -= InitiateWorldFlip;
        }

        private void Update()
        {
            switch (m_worldState)
            {
                case WorldState.ControllerControlled:
                {
                    foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
                    {
                        bool clickedInPlace = cubeLayerObjectV2.UpdateRotations();
                        if (clickedInPlace)
                        {
                            OnWorldClicked?.Invoke();
                            //m_playerGridController.ResetPlayerGravityState();
                            Dummy.Instance.RotateTendingDirection();
                            if (Dummy.Instance.IsPlayerStuck())
                            {
                                StartCoroutine(Dummy.Instance.MoveToCubie(Dummy.Instance.tendingDirection));
                            }

                            audioController.PlaySound(AudioController.AudioEnum.GearClick);
                        }

                        UpdatePlayerMovementState();
                    }
                }
                    break;

                case WorldState.FlipTile:
                case WorldState.FlipWorld:
                {
                    m_lerpAmount += lerpSpeed * Time.deltaTime;
                    Vector3 currentRotation = Vector3.Lerp(
                        m_startRotation,
                        m_targetRotation,
                        m_lerpAmount
                    );
                    m_flipTarget.rotation = Quaternion.Euler(currentRotation);
                    if (m_lerpAmount >= lerpEndingAmount)
                    {
                        m_flipTarget.rotation = Quaternion.Euler(m_targetRotation);

                        if (m_worldState == WorldState.FlipTile)
                        {
                            StartWorldFlip();
                        }
                        else
                        {
                            EndWorldFlip();
                        }
                    }
                }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void CheckAndUpdateRotation(CubeLayerMaskV2 i_cubeLayerMask, int i_direction)
        {
            if (m_worldState != WorldState.ControllerControlled)
            {
                return;
            }

            bool isRotating = false;
            cubeLayers.ForEach(_ => isRotating |= _.IsRotating);
            if (isRotating && !CubeLayerMaskV2.IsCombinedNotZero(m_lastLayerMask, i_cubeLayerMask))
            {
                return;
            }

            if (Dummy.Instance.IsPlayerMoving() &&
                CubeLayerMaskV2.IsCombinedNotZero(new CubeLayerMaskV2(Dummy.Instance.pendingDirection), i_cubeLayerMask))
            {
                foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
                {
                    if (cubeLayerObjectV2.cubeLayerMask == i_cubeLayerMask)
                    {
                        if (cubeLayerObjectV2.IsInside(Dummy.Instance.GetCurrentCubie().gameObject)) return;
                        if (cubeLayerObjectV2.IsInside(Dummy.Instance.m_movingTarget)) return;
                        break;
                    }
                }
            }

            m_lastLayerMask = i_cubeLayerMask;

            int finalRotationDelta = rotationDelta * i_direction;
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                bool wasParentCreated = cubeLayerObjectV2.CheckAndCreateParent(i_cubeLayerMask, finalRotationDelta);
                if (wasParentCreated)
                {
                    audioController.PlaySound(AudioController.AudioEnum.GearTurning);
                }
            }
        }

        public bool IsMovementAllowed => m_isMovementAllowed;

        #endregion

        #region Utility Functions

        private void UpdatePlayerMovementState()
        {
            bool isMovementAllowed = true;
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                if (cubeLayerObjectV2.IsRotating)
                {
                    isMovementAllowed = false;
                    break;
                }
            }

            m_isMovementAllowed = isMovementAllowed;

            if (isMovementAllowed)
            {
                Dummy.Instance.AllowPlayerMovement();
            }
            else
            {
                Dummy.Instance.PreventPlayerMovement();
            }
        }

        private void InitiateWorldFlip(Transform parentTransform)
        {
            Debug.Log("Starting Tile Flip");

            //m_playerGridController.PreventPlayerMovement();
            Dummy.Instance.PreventPlayerMovement();

            //m_playerPreviousParent = m_playerGridController.transform.parent;
            //m_playerGridController.transform.SetParent(parentTransform);
            m_playerPreviousParent = Dummy.Instance.transform.parent;
            m_playerPreviousParent.transform.SetParent(parentTransform);

            m_startRotation = parentTransform.rotation.eulerAngles;
            m_targetRotation = parentTransform.rotation.eulerAngles * -1;
            m_lerpAmount = 0;
            m_flipTarget = parentTransform;

            SetWorldState(WorldState.FlipTile);
        }

        private void StartWorldFlip()
        {
            Debug.Log("Starting World Flip");

            m_startRotation = worldRoot.rotation.eulerAngles;
            m_targetRotation = worldRoot.rotation.eulerAngles + new Vector3(180, 0, 0);
            m_lerpAmount = 0;
            m_flipTarget = worldRoot;

            SetWorldState(WorldState.FlipWorld);
        }

        private void EndWorldFlip()
        {
            Debug.Log("Ending World Flip");

            m_isPlayerOutside = !m_isPlayerOutside;

            //m_playerGridController.AllowPlayerMovement();
            //m_playerGridController.transform.SetParent(m_playerPreviousParent);

            Dummy.Instance.AllowPlayerMovement();
            Dummy.Instance.transform.SetParent(m_playerPreviousParent);

            if (m_isPlayerOutside)
            {
                layerPlayerFollower.SetFollowActive();

                foreach (GameObject centerBlock in centerBlocks)
                {
                    centerBlock.SetActive(true);
                }

                // outsideWorld.SetActive(true);
            }
            else
            {
                layerPlayerFollower.DeactivateFollow();
                layerPlayerFollower.SetLayerDefaultPosition();

                cameraController.SetCameraDefaultPosition();

                foreach (GameObject centerBlock in centerBlocks)
                {
                    centerBlock.SetActive(false);
                }

                // outsideWorld.SetActive(false);
            }

            SetWorldState(WorldState.ControllerControlled);
        }

        private void SetWorldState(WorldState i_worldState) => m_worldState = i_worldState;

        private void SetupInitialWorldState()
        {
            foreach (GameObject centerBlock in centerBlocks)
            {
                centerBlock.SetActive(false);
            }

            // outsideWorld.SetActive(false);
        }

        #endregion

        #region Enums

        private enum WorldState
        {
            ControllerControlled,
            FlipTile,
            FlipWorld
        }

        #endregion
    }
}