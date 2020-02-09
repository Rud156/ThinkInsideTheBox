using Player;
using UnityEngine;

namespace WorldCube
{
    public class CubeLayerPlayerFollower : MonoBehaviour
    {
        public PlayerGridPositionMarker playerGridPositionMarker;

        private bool m_isFollowActive;

        #region Unity Functions

        private void Update()
        {
            if (!m_isFollowActive)
            {
                return;
            }

            Vector3 followPosition = playerGridPositionMarker.GridPosition;
            transform.position = followPosition;
        }

        #endregion

        #region External Function

        public void SetFollowActive() => m_isFollowActive = true;

        public void DeactivateFollow() => m_isFollowActive = false;

        #endregion
    }
}