using UnityEngine;

namespace WorldCube
{
    public class CubeController : MonoBehaviour
    {
        private const string CenterBlock = "CenterBlock";
        private const string ParentCube = "SideCubeParent";

        #region Structs

        [System.Serializable]
        public struct CubeSides
        {
            [Header("Positions")] public Transform centerCube;
            public Transform rayCastPosition; // Can Be Removed and Calculated

            [Header("Direction and Distance")] public Vector3 raycastDirection;
        }

        [System.Serializable]
        public struct FakeParentData
        {
            public GameObject parent;
            public Vector3 targetRotation;
            public int arrayIndex;
        }

        #endregion
    }
}