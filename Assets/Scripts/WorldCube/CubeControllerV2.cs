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
        private CubeLayerMaskV2 m_lastLayerMask;

        #region Unity Functions

        private void Start()
        {
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                cubeLayerObjectV2.SetTileDistance(tileDistance);
            }
        }

        private void Update()
        {
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in cubeLayers)
            {
                cubeLayerObjectV2.UpdateRotations();
            }

            UpdatePlayerMovementState();
        }

        #endregion

        #region External Functions

        public void CheckAndUpdateRotation(CubeLayerMaskV2 i_cubeLayerMask, int i_direction)
        {
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

        #endregion
    }
}