using UnityEngine;

namespace WorldCube
{
    public class CubeLayerPlayerFollower : MonoBehaviour
    {
        public Transform playerTransform;
        public Vector3 positionOffset;

        #region Unity Functions

        private void Update()
        {
            Vector3 targetPosition = playerTransform.position;
            targetPosition += positionOffset;

            transform.position = targetPosition;
        }

        #endregion
    }
}