using UnityEngine;

namespace Player
{
    public class PlayerGridControllerV2 : MonoBehaviour
    {
        [Header("RayCasts")] public float rayCastYDistance;
        public float distanceYTolerance;
        public float distanceYMovementSpeed; // Only used for falling

        [Header("Rotation")] public float yRotationSpeed;

        [Header("Movement")] public float movementLerpSpeed;
    }
}