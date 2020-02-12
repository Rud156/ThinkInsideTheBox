using UnityEngine;

namespace Camera
{
    // TODO: Add Camera Shake
    public class CameraController : MonoBehaviour
    {
        [Header("Positions")] public Transform cameraDefaultTransform;
        public Vector3 playerFollowOffset;
        public Transform playerTransform;

        [Header("Lerping")] public float lerpSpeed;
        public AnimationCurve lerpCurve;

        private bool m_followPlayer;

        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private float m_lerpAmount;

        #region Unity Functions

        private void Start()
        {
            m_followPlayer = false;
            m_startPosition = cameraDefaultTransform.position;
            m_targetPosition = cameraDefaultTransform.position;
            m_lerpAmount = 1;
        }

        private void Update()
        {
            if (m_followPlayer)
            {
                Vector3 playerPosition = playerTransform.position + playerFollowOffset;
                if (m_targetPosition != playerPosition)
                {
                    m_targetPosition = playerPosition;
                    m_startPosition = transform.position;
                    m_lerpAmount = 0;
                }
            }

            if (m_lerpAmount < 1)
            {
                m_lerpAmount += lerpSpeed * Time.deltaTime;
            }

            transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, lerpCurve.Evaluate(m_lerpAmount));
        }

        #endregion

        #region External Functions

        public void SetCameraDefaultPosition()
        {
            m_targetPosition = cameraDefaultTransform.position;
            m_startPosition = transform.position;
            m_lerpAmount = 0;
        }

        public void SetFollowActive() => m_followPlayer = true;

        public void DeactivateFollow() => m_followPlayer = false;

        #endregion

        #region Utility Functions

        #endregion
    }
}