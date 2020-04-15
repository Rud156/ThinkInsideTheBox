using System.Collections.Generic;
using UnityEngine;
using CubeData;

namespace WorldCube
{
    public class CubeLayerObjectV2 : MonoBehaviour
    {
        private const int RotationLocker = 90;
        public CubeLayerMaskV2 cubeLayerMask;
        public float collisionRadius = 0.5f;
        public float lerpChangeRate = 1;
        public float lerpEndingAmount = 0.97f;
        public LayerMask layerMask;
        public LayerMask displayShaderLayerMask;
        private List<CubieObject> m_childCubies = new List<CubieObject>();
        private float m_currentSideRotation;
        private float m_startRotation;
        private int m_targetRotation;
        private float m_lerpAmount;
        private int m_tileDistance;

        #region External Functions

        // Returns whether objects clicked into place or not
        public bool UpdateRotations()
        {
            // This means that all children have been released and thus no rotation is possible
            if (m_childCubies.Count == 0)
            {
                return false;
            }

            float lerpAmount = m_lerpAmount + lerpChangeRate * Time.deltaTime;
            m_lerpAmount = lerpAmount;
            float updatedSideRotation = Mathf.Lerp(
                m_startRotation,
                m_targetRotation,
                m_lerpAmount
            );
            m_currentSideRotation = updatedSideRotation;
            Vector3 currentRotation = GetVectorRotation(m_currentSideRotation);
            transform.rotation = Quaternion.Euler(currentRotation);
            if (m_lerpAmount >= lerpEndingAmount && m_targetRotation % RotationLocker == 0)
            {
                m_currentSideRotation = m_targetRotation;
                m_startRotation = m_targetRotation;
                m_lerpAmount = 0;
                Vector3 finalRotation = GetVectorRotation(m_currentSideRotation);
                transform.rotation = Quaternion.Euler(finalRotation);
                ReleaseChildren();
                return true;
            }

            return false;
        }

        public bool CheckAndCreateParent(CubeLayerMaskV2 i_cubeLayerMask, int i_rotationDelta)
        {
            if (i_cubeLayerMask != cubeLayerMask)
            {
                return false;
            }

            int updatedTargetRotation = m_targetRotation + i_rotationDelta;
            // Only in this case create and add children
            if (m_currentSideRotation % RotationLocker == 0 && updatedTargetRotation % RotationLocker != 0 &&
                !HasChildren())
            {
                bool areChildrenValid = GrabAndValidateChildren();
                // This means that some other layer has control over these children
                if (!areChildrenValid)
                {
                    m_childCubies.Clear();
                    return false;
                }
            }

            m_targetRotation = updatedTargetRotation;
            m_startRotation = m_currentSideRotation;
            m_lerpAmount = 0;
            return true;
        }

        public void SetTileDistance(int distance) => m_tileDistance = distance;

        public bool IsRotating => m_currentSideRotation != m_targetRotation ||
                                  m_targetRotation % RotationLocker != 0;

        #endregion

        #region Utility Functions

        private bool GrabAndValidateChildren()
        {
            m_childCubies.Clear();
            for (int i = -m_tileDistance; i <= m_tileDistance; i += m_tileDistance)
            {
                for (int j = -m_tileDistance; j <= m_tileDistance; j += m_tileDistance)
                {
                    float xValue = 0;
                    float yValue = 0;
                    float zValue = 0;
                    if (Mathf.Abs(cubeLayerMask.X) == 1)
                    {
                        xValue = 0;
                        yValue = i;
                        zValue = j;
                    }
                    else if (Mathf.Abs(cubeLayerMask.Y) == 1)
                    {
                        xValue = i;
                        yValue = 0;
                        zValue = j;
                    }
                    else if (Mathf.Abs(cubeLayerMask.Z) == 1)
                    {
                        xValue = i;
                        yValue = j;
                        zValue = 0;
                    }

                    Vector3 finalPosition = transform.position + new Vector3(xValue, yValue, zValue);
                    Collider[] other = Physics.OverlapSphere(finalPosition, collisionRadius, layerMask);
                    // This basically checks and gets all cubes that lie in the position
                    List<CubieObject> data = GetColliderCube(other);
                    bool isInvalidData = false;
                    foreach (CubieObject cubeieDataV2 in data)
                    {
                        if (cubeieDataV2.HasParent)
                        {
                            isInvalidData = true;
                            break;
                        }
                    }

                    if (isInvalidData || data.Count < 1)
                    {
                        continue;
                    }

                    // Ideally this count should never be more than 2
                    // Add a check probably
                    foreach (CubieObject cubeieDataV2 in data)
                    {
                        if(!cubeieDataV2.CompareTag("Exit"))
                        {
                            cubeieDataV2.SetParent(cubeLayerMask, transform);
                            m_childCubies.Add(cubeieDataV2);
                        }
                        
                    }
                }
            }

            if (m_childCubies.Count == 0)
            {
                Debug.LogError("No Cubies Found");
                return false;
            }

            return true;
        }

        private void ReleaseChildren()
        {
            for (int i = 0; i < m_childCubies.Count; i++)
            {
                m_childCubies[i].ReleaseParent(cubeLayerMask);
            }

            m_childCubies.Clear();
        }

        private bool HasChildren() => m_childCubies.Count != 0;

        private List<CubieObject> GetColliderCube(Collider[] other)
        {
            List<CubieObject> cubeData = new List<CubieObject>();
            for (int i = 0; i < other.Length; i++)
            {
                CubieObject data = other[i].transform.GetComponent<CubieObject>();
                if (data)
                {
                    cubeData.Add(data);
                }
            }

            return cubeData;
        }

        private Vector3 GetVectorRotation(float rotation)
        {
            return new Vector3(
                rotation * cubeLayerMask.X,
                rotation * cubeLayerMask.Y,
                rotation * cubeLayerMask.Z
            );
        }

        public bool IsInside(GameObject i_other)
        {
            return GetComponent<Collider>().bounds.Contains(i_other.transform.position);
        }

        #endregion
    }
}