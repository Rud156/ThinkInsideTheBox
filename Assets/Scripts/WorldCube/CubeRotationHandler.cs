using System;
using CubeData;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeRotationHandler : MonoBehaviour
    {
        private const int RotationLocker = 90;

        private FaceDirection[] m_initialCenterRow;
        private FaceDirection[] m_currentCenterRow;

        #region Unity Functions

        private void Start()
        {
            m_initialCenterRow = new[] {FaceDirection.Left, FaceDirection.Back, FaceDirection.Right, FaceDirection.Front};
            m_currentCenterRow = new FaceDirection[4];
            Array.Copy(m_initialCenterRow, m_currentCenterRow, 4);
        }

        #endregion

        #region External Functions

        public void RotateCenter(float i_yValue)
        {
            int n = m_initialCenterRow.Length; // Always will be 4
            i_yValue = ExtensionFunctions.GetClosestMultiple(ExtensionFunctions.To360Angle(i_yValue), RotationLocker);

            int yTimes = (int) i_yValue / RotationLocker;
            yTimes = yTimes % n;

            int yStartPosition = Mathf.Abs(n - yTimes);
            int iY = 0;

            for (int j = yStartPosition; j < n; j++)
            {
                m_currentCenterRow[iY] = m_initialCenterRow[j];
                iY += 1;
            }

            for (int j = 0; j < yStartPosition; j++)
            {
                m_currentCenterRow[iY] = m_initialCenterRow[j];
                iY += 1;
            }
        }

        public FaceDirection Left => m_currentCenterRow[0];

        public FaceDirection Back => m_currentCenterRow[1];

        public FaceDirection Right => m_currentCenterRow[2];

        public FaceDirection Forward => m_currentCenterRow[3];

        public CubeLayerMask GetCubeLayerMask(FaceDirection i_faceDirection)
        {
            switch (i_faceDirection)
            {
                case FaceDirection.Top:
                    return CubeLayerMask.up;

                case FaceDirection.Left:
                    return CubeLayerMask.left;

                case FaceDirection.Back:
                    return CubeLayerMask.back;

                case FaceDirection.Right:
                    return CubeLayerMask.right;

                case FaceDirection.Front:
                    return CubeLayerMask.forward;

                case FaceDirection.Bottom:
                    return CubeLayerMask.down;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_faceDirection), i_faceDirection, null);
            }
        }

        public CubeLayerMaskV2 GetCubeLayerMaskV2(FaceDirection i_faceDirection)
        {
            switch (i_faceDirection)
            {
                case FaceDirection.Top:
                    return CubeLayerMaskV2.Up;

                case FaceDirection.Left:
                    return CubeLayerMaskV2.Left;

                case FaceDirection.Back:
                    return CubeLayerMaskV2.Back;

                case FaceDirection.Right:
                    return CubeLayerMaskV2.Right;

                case FaceDirection.Front:
                    return CubeLayerMaskV2.Forward;

                case FaceDirection.Bottom:
                    return CubeLayerMaskV2.Down;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_faceDirection), i_faceDirection, null);
            }
        }

        #endregion

        #region Enums

        public enum FaceDirection
        {
            None,
            Top,
            Left,
            Back,
            Right,
            Front,
            Bottom
        }

        #endregion
    }
}