using System;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeRotationHandler : MonoBehaviour
    {
        private const int RotationLocker = 90;

        private FaceDirection[] m_initialCenterRow;
        private FaceDirection m_initialTopFace;
        private FaceDirection m_initialBottomFace;

        private FaceDirection[] m_currentCenterRow;
        private FaceDirection m_currentTopFace;
        private FaceDirection m_currentBottomFace;

        #region Unity Functions

        private void Start()
        {
            m_initialCenterRow = new[] {FaceDirection.Left, FaceDirection.Back, FaceDirection.Right, FaceDirection.Front};
            m_initialTopFace = FaceDirection.Top;
            m_initialBottomFace = FaceDirection.Bottom;

            m_currentCenterRow = new[] {FaceDirection.None, FaceDirection.None, FaceDirection.None, FaceDirection.None};
            m_currentTopFace = FaceDirection.None;
            m_currentBottomFace = FaceDirection.None;
        }

        #endregion

        #region External Functions

        public void Rotate(float i_xValue, float i_yValue, float i_zValue)
        {
            int n = m_initialCenterRow.Length; // Always will be 4
            i_xValue = ExtensionFunctions.To360Angle(i_xValue);
            i_yValue = ExtensionFunctions.To360Angle(i_yValue);
            i_zValue = ExtensionFunctions.To360Angle(i_zValue);

            #region Rotate The Center Row

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

            #endregion

            #region Rotate The Z Axis Row

            FaceDirection[] zNewArray = new[] {m_currentCenterRow[0], m_initialTopFace, m_currentCenterRow[2], m_initialBottomFace};
            FaceDirection[] zNewTempArray = new[] {FaceDirection.None, FaceDirection.None, FaceDirection.None, FaceDirection.None};

            int zTimes = (int) i_zValue / RotationLocker;
            zTimes = zTimes % n;

            int zStartPosition = Mathf.Abs(n - zTimes);
            int iZ = 0;

            for (int j = zStartPosition; j < n; j++)
            {
                zNewTempArray[iZ] = zNewArray[j];
                iZ += 1;
            }

            for (int j = 0; j < zStartPosition; j++)
            {
                zNewTempArray[iZ] = zNewArray[j];
                iZ += 1;
            }

            m_currentCenterRow[0] = zNewTempArray[0];
            m_currentCenterRow[2] = zNewTempArray[2];
            m_currentTopFace = zNewTempArray[1];
            m_currentBottomFace = zNewTempArray[3];

            #endregion

            #region Rotate The X Axis Row

            FaceDirection[] xNewArray = new[] {m_currentCenterRow[1], m_currentTopFace, m_currentCenterRow[3], m_currentBottomFace};
            FaceDirection[] xNewTempArray = new[] {FaceDirection.None, FaceDirection.None, FaceDirection.None, FaceDirection.None};

            int xTimes = (int) i_xValue / RotationLocker;
            xTimes = xTimes % n;

            int xStartPosition = Mathf.Abs(n - xTimes);
            int iX = 0;

            for (int j = xStartPosition; j < n; j++)
            {
                xNewTempArray[iX] = xNewArray[j];
                iX += 1;
            }

            for (int j = 0; j < xStartPosition; j++)
            {
                xNewTempArray[iX] = xNewArray[j];
                iX += 1;
            }

            m_currentCenterRow[1] = xNewTempArray[0];
            m_currentCenterRow[3] = xNewTempArray[2];
            m_currentTopFace = xNewTempArray[1];
            m_currentBottomFace = xNewTempArray[3];

            #endregion
        }

        public FaceDirection Left => m_currentCenterRow[0];

        public FaceDirection Back => m_currentCenterRow[1];

        public FaceDirection Right => m_currentCenterRow[2];

        public FaceDirection Front => m_currentCenterRow[3];

        public FaceDirection Top => m_currentTopFace;

        public FaceDirection Bottom => m_currentBottomFace;

        public CubeLayerMaskV2 GetCubeLayerMask(FaceDirection i_faceDirection)
        {
            switch (i_faceDirection)
            {
                case FaceDirection.Top:
                    return new CubeLayerMaskV2(0, 1, 0);

                case FaceDirection.Left:
                    return new CubeLayerMaskV2(1, 0, 0);

                case FaceDirection.Back:
                    return new CubeLayerMaskV2(0, 0, -1);

                case FaceDirection.Right:
                    return new CubeLayerMaskV2(-1, 0, 0);

                case FaceDirection.Front:
                    return new CubeLayerMaskV2(0, 0, 1);

                case FaceDirection.Bottom:
                    return new CubeLayerMaskV2(0, -1, 0);

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