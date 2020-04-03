using UnityEngine;
using UnityEngine.Serialization;

namespace WorldCube
{
    public class CubeSideTouchIndicator : MonoBehaviour
    {
        [Header("Side")] public CubeInputTouchController.Side side1;
        public CubeInputTouchController.Side side2;

        [Header("Touch")] public CubeInputTouchController.MovementDirection touchMovementDirection1;
        public CubeInputTouchController.MovementDirection touchMovementDirection2;

        [Header("Multiplier")] public int movementDirectionMultiplier1;
        public int movementDirectionMultiplier2;

        public CubeInputTouchController.Side GetSideForMovement(CubeInputTouchController.MovementDirection i_movementDirection)
        {
            if (i_movementDirection == touchMovementDirection1)
            {
                return side1;
            }
            else if (i_movementDirection == touchMovementDirection2)
            {
                return side2;
            }
            else
            {
                return CubeInputTouchController.Side.None;
            }
        }

        public int GetDirectionMultiplierForMovement(CubeInputTouchController.MovementDirection i_movementDirection)
        {
            if (i_movementDirection == touchMovementDirection1)
            {
                return movementDirectionMultiplier1;
            }
            else if (i_movementDirection == touchMovementDirection2)
            {
                return movementDirectionMultiplier2;
            }
            else
            {
                return 0;
            }
        }
    }
}