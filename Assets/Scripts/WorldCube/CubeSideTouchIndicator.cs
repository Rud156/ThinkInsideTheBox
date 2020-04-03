using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeSideTouchIndicator : MonoBehaviour
    {
        private const int RotationLocker = 90;

        [Header("Side")] public CubeInputTouchController.Side side1;
        public CubeInputTouchController.Side side2;

        [Header("Touch")] public CubeInputTouchController.MovementDirection touchMovementDirection1;
        public CubeInputTouchController.MovementDirection touchMovementDirection2;

        [Header("Multiplier")] public int movementDirectionMultiplier1;
        public int movementDirectionMultiplier2;

        [Header("Modifiers")] public bool useRotationMultiplier;
        public Transform mainCameraHolderTransform;

        #region External Functions

        public CubeInputTouchController.Side GetSideForMovement(CubeInputTouchController.MovementDirection i_movementDirection)
        {
            if (useRotationMultiplier)
            {
                float yValue = mainCameraHolderTransform.eulerAngles.y;
                int yValueNormalized = ExtensionFunctions.GetClosestMultiple(ExtensionFunctions.To360Angle(yValue), RotationLocker);
                return GetRotatedSideMovement(i_movementDirection, yValueNormalized);
            }
            else
            {
                return GetDefaultSideMovement(i_movementDirection);
            }
        }

        public int GetDirectionMultiplierForMovement(CubeInputTouchController.MovementDirection i_movementDirection)
        {
            if (useRotationMultiplier)
            {
                float yValue = mainCameraHolderTransform.eulerAngles.y;
                int yValueNormalized = ExtensionFunctions.GetClosestMultiple(ExtensionFunctions.To360Angle(yValue), RotationLocker);
                return GetRotatedDirectionMultiplier(i_movementDirection, yValueNormalized);
            }
            else
            {
                return GetDefaultDirectionMultiplier(i_movementDirection);
            }
        }

        #endregion

        #region Utility Functions

        private CubeInputTouchController.Side GetDefaultSideMovement(CubeInputTouchController.MovementDirection i_movementDirection)
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

        private int GetDefaultDirectionMultiplier(CubeInputTouchController.MovementDirection i_movementDirection)
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

        private CubeInputTouchController.Side GetRotatedSideMovement(CubeInputTouchController.MovementDirection i_movementDirection, int i_rotationAngle)
        {
            // There probably is a better way to do this.
            // But Shit works. I guess
            switch (i_rotationAngle)
            {
                // As they are the same...
                case 0:
                case 180:
                case 360:
                    return GetDefaultSideMovement(i_movementDirection);

                case 90:
                case 270:
                {
                    // Basically flip movements for these angles...
                    if (i_movementDirection != touchMovementDirection1)
                    {
                        return side1;
                    }
                    else if (i_movementDirection != touchMovementDirection2)
                    {
                        return side2;
                    }
                    else
                    {
                        return CubeInputTouchController.Side.None;
                    }
                }

                default:
                    return CubeInputTouchController.Side.None;
            }
        }

        private int GetRotatedDirectionMultiplier(CubeInputTouchController.MovementDirection i_movementDirection, int i_rotationAngle)
        {
            // There probably is a better way to do this.
            // But Shit works. I guess
            switch (i_rotationAngle)
            {
                // As they are the same...
                case 0:
                case 360:
                    return GetDefaultDirectionMultiplier(i_movementDirection);

                case 180:
                {
                    if (i_movementDirection == touchMovementDirection1)
                    {
                        return -movementDirectionMultiplier1;
                    }
                    else if (i_movementDirection == touchMovementDirection2)
                    {
                        return -movementDirectionMultiplier2;
                    }
                    else
                    {
                        return 0;
                    }
                }

                // Basically flip movements for these angles...
                case 90:
                {
                    // Specific fix for these 2 sides...
                    // Generally a bad idea...
                    CubeInputTouchController.Side side = GetRotatedSideMovement(i_movementDirection, i_rotationAngle);
                    if (side == CubeInputTouchController.Side.Left || side == CubeInputTouchController.Side.Right)
                    {
                        if (i_movementDirection != touchMovementDirection1)
                        {
                            return -movementDirectionMultiplier1;
                        }
                        else if (i_movementDirection != touchMovementDirection2)
                        {
                            return -movementDirectionMultiplier2;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (i_movementDirection != touchMovementDirection1)
                        {
                            return movementDirectionMultiplier1;
                        }
                        else if (i_movementDirection != touchMovementDirection2)
                        {
                            return movementDirectionMultiplier2;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }


                case 270:
                {
                    CubeInputTouchController.Side side = GetRotatedSideMovement(i_movementDirection, i_rotationAngle);
                    if (side == CubeInputTouchController.Side.Left || side == CubeInputTouchController.Side.Right)
                    {
                        if (i_movementDirection != touchMovementDirection1)
                        {
                            return movementDirectionMultiplier1;
                        }
                        else if (i_movementDirection != touchMovementDirection2)
                        {
                            return movementDirectionMultiplier2;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (i_movementDirection != touchMovementDirection1)
                        {
                            return -movementDirectionMultiplier1;
                        }
                        else if (i_movementDirection != touchMovementDirection2)
                        {
                            return -movementDirectionMultiplier2;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }

                default:
                    return 0;
            }
        }

        #endregion
    }
}