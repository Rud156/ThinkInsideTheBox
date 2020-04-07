using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerGridPositionMarker : MonoBehaviour
    {
        [Header("Transforms")] public Vector3 gridStartTransform;

        [Header("Position Info")] public int sideSize;

        private Transform m_playerTransform;
        private Vector3 m_currentGridCenter;

        #region Unity Functions

        private void Start() => m_playerTransform = GameObject.FindGameObjectWithTag(TagManager.Player).transform;

        private void Update()
        {
            Vector3 playerPosition = m_playerTransform.position;

            float xValue = ExtensionFunctions.GetClosestMultiple(playerPosition.x, 2);
            float yValue = ExtensionFunctions.GetClosestMultiple(playerPosition.y, 2);
            float zValue = ExtensionFunctions.GetClosestMultiple(playerPosition.z, 2);

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