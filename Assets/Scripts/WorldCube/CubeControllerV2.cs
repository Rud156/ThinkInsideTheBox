using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace WorldCube
{
    public class CubeControllerV2 : MonoBehaviour
    {
        [Header("Position Data")] public int tileDistance;
        public int rotationDelta;

        [Header("Player")] public PlayerGridController playerGridController;

        [Header("Layers")] public List<CubeLayerObjectV2> cubeLayers;

        [Header("World Flip")] public Transform worldRoot;
        public float lerpSpeed;
        public float lerpEndingAmount = 0.97f;

        private CubeLayerMaskV2 m_lastLayerMask;

        // World Flip
        private Transform m_playerPreviousParent;
        private Transform m_flipTarget;
        private Vector3 m_startRotation;
        private Vector3 m_targetRotation;
        private float m_lerpAmount;

        private WorldState m_worldState;

        #region Unity Functions

        private void Start()
        {
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                cubeLayerObjectV2.SetTileDistance(tileDistance);
            }

            SetWorldState(WorldState.ControllerControlled);
            playerGridController.OnWorldFlip += InitiateWorldFlip;
        }

        private void OnDestroy()
        {
            playerGridController.OnWorldFlip -= InitiateWorldFlip;
        }

        private void Update()
        {
            switch (m_worldState)
            {
                case WorldState.ControllerControlled:
                {
                    foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
                    {
                        cubeLayerObjectV2.UpdateRotations();
                    }

                    UpdatePlayerMovementState();
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

            m_lastLayerMask = i_cubeLayerMask;

            int finalRotationDelta = rotationDelta * i_direction;
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                cubeLayerObjectV2.CheckAndCreateParent(i_cubeLayerMask, finalRotationDelta);
            }
        }

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

            if (isMovementAllowed)
            {
                playerGridController.AllowPlayerMovement();
            }
            else
            {
                playerGridController.PreventPlayerMovement();
            }
        }

        private void InitiateWorldFlip(Transform parentTransform)
        {
            Debug.Log("Starting Tile Flip");

            playerGridController.PreventPlayerMovement();

            m_playerPreviousParent = playerGridController.transform.parent;
            playerGridController.transform.SetParent(parentTransform);

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

            playerGridController.AllowPlayerMovement();
            playerGridController.transform.SetParent(m_playerPreviousParent);

            SetWorldState(WorldState.ControllerControlled);
        }

        private void SetWorldState(WorldState i_worldState) => m_worldState = i_worldState;

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