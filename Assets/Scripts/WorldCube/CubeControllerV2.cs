using System.Collections.Generic;
using UnityEngine;

namespace WorldCube
{
    public class CubeControllerV2 : MonoBehaviour
    {
        public int tileDistance;
        public int rotationDelta;

        private List<CubeLayerObjectV2> m_cubeLayers;
        private CubeLayerMaskV2 m_lastLayerMask;

        #region Unity Functions

        private void Start()
        {
            m_cubeLayers = new List<CubeLayerObjectV2>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -tileDistance; j <= tileDistance; j += tileDistance)
                {
                    for (int k = -tileDistance; k <= tileDistance; k += tileDistance)
                    {
                        float xValue = j;
                        float yValue = k;
                        float zValue = i * tileDistance;

                        CubeLayerObjectV2 cubeLayerObject = new GameObject("Layer").AddComponent<CubeLayerObjectV2>();
                        cubeLayerObject.SetTileDistance(tileDistance);
                        m_cubeLayers.Add(cubeLayerObject);
                    }
                }
            }
        }

        private void Update()
        {
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in m_cubeLayers)
            {
                cubeLayerObjectV2.UpdateRotations();
            }
        }

        #endregion

        #region External Functions

        public void CheckAndUpdateRotation(CubeLayerMaskV2 i_cubeLayerMask, int i_direction)
        {
            bool isRotating = false;
            m_cubeLayers.ForEach(_ => isRotating |= _.IsRotating);
            if (isRotating && !CubeLayerMaskV2.IsCombinedNotZero(m_lastLayerMask, i_cubeLayerMask))
            {
                return;
            }

            m_lastLayerMask = i_cubeLayerMask;

            int finalRotationDelta = rotationDelta * i_direction;
            foreach (CubeLayerObjectV2 cubeLayerObjectV2 in m_cubeLayers)
            {
                cubeLayerObjectV2.CheckAndCreateParent(i_cubeLayerMask, finalRotationDelta);
            }
        }

        #endregion
    }
}