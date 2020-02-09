using Player;
using UnityEngine;

namespace WorldCube
{
    public class CubeLayerPlayerFollower : MonoBehaviour
    {
        public PlayerGridPositionMarker playerGridPositionMarker;

        #region Unity Functions

        private void Update()
        {
            Vector3 followPosition = playerGridPositionMarker.GridPosition;
            transform.position = followPosition;
        }

        #endregion
    }
}