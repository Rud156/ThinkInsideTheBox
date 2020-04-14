using Player;
using UnityEngine;

namespace WorldCube
{
    public class CubeLayerPlayerFollower : MonoBehaviour
    {
        [Header("Positions")] public Transform layerDefaultTransform;

        [Header("Player")] public PlayerGridPositionMarker playerGridPositionMarker;
        public bool followPlayer;

        [Header("Lerping")] public float lerpSpeed;
        public AnimationCurve lerpCurve;

        private bool m_followPlayer;

        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private float m_lerpAmount;

        #region Unity Functions

        private void Start()
        {
            m_followPlayer = followPlayer;
            m_startPosition = layerDefaultTransform.position;
            m_targetPosition = layerDefaultTransform.position;
            m_lerpAmount = 1;
        }

        private void Update()
        {
            if (m_followPlayer)
            {
                Vector3 followPosition = playerGridPositionMarker.GridPosition;
                if (m_targetPosition != followPosition)
                {
                    m_startPosition = transform.position;
                    m_targetPosition = followPosition;
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

        #region External Function

        public void SetLayerDefaultPosition()
        {
            m_targetPosition = layerDefaultTransform.position;
            m_startPosition = transform.position;
            m_lerpAmount = 0;
        }

        public bool ReachedPosition() => m_lerpAmount >= 1;

        public void SetFollowActive() => m_followPlayer = true;

        public void DeactivateFollow() => m_followPlayer = false;

        #endregion
    }
}