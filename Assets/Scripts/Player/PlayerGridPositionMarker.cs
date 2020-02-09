using Extensions;
using UnityEngine;

namespace Player
{
    public class PlayerGridPositionMarker : MonoBehaviour
    {
        [Header("Transforms")] public Vector3 gridStartTransform;
        public Transform playerTransform;

        [Header("Position Info")] public int sideSize;

        private Vector3 m_currentGridCenter;

        #region Unity Functions

        private void Update()
        {
            Vector3 playerPosition = playerTransform.position;

            float xValue = ExtensionFunctions.GetClosestMultiple(playerPosition.x);
            float yValue = ExtensionFunctions.GetClosestMultiple(playerPosition.y);
            float zValue = ExtensionFunctions.GetClosestMultiple(playerPosition.z);

            m_currentGridCenter.Set(xValue, yValue, zValue);
            m_currentGridCenter -= gridStartTransform;

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log(m_currentGridCenter);
            }
        }

        #endregion

        #region External Functions

        public Vector3 GridPosition => m_currentGridCenter;

        #endregion
    }
}