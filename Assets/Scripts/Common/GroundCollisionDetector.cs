using UnityEngine;

namespace Common
{
    public class GroundCollisionDetector : MonoBehaviour
    {
        [Header("RayCast Data")] public float rayCastDistance;
        public Vector3 rayCastPlayerPositionOffset;
        public LayerMask layerMask;

        [Header("Debug")] public bool isDebugActive;
        public Color rayCastColor = Color.red;
        public float rayCastTime;

        public delegate void PlayerLanded();

        public PlayerLanded OnPlayerLanded;

        private bool m_lastGroundStatus;
        private bool m_isPlayerOnGround;

        #region Unity Functions

        private void Update()
        {
            Vector3 rayCastPosition = transform.position + rayCastPlayerPositionOffset;

            if (Physics.Raycast(rayCastPosition, Vector3.down, out RaycastHit hit, rayCastDistance, layerMask))
            {
                // Maybe use hit for something later on...
                m_isPlayerOnGround = true;
            }
            else
            {
                m_isPlayerOnGround = false;
            }

            CheckAndNotifyPlayerLanded();
            HandleDebug(rayCastPosition);

            m_lastGroundStatus = m_isPlayerOnGround;
        }

        #endregion

        #region External Functions

        public bool IsPlayerOnGround => m_isPlayerOnGround;

        #endregion

        #region Utility Functions

        private void CheckAndNotifyPlayerLanded()
        {
            if (m_lastGroundStatus != m_isPlayerOnGround && m_isPlayerOnGround)
            {
                OnPlayerLanded?.Invoke();
            }
        }

        private void HandleDebug(Vector3 rayCastPosition)
        {
            if (!isDebugActive)
            {
                return;
            }

            Debug.DrawRay(rayCastPosition, Vector3.down * rayCastDistance, rayCastColor, rayCastTime);
        }

        #endregion
    }
}