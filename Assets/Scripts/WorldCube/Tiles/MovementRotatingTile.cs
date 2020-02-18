using System;
using UnityEngine;

namespace WorldCube.Tile
{
    public class MovementRotatingTile : MonoBehaviour
    {
        public TileRotationAxis tileRotationAxis = TileRotationAxis.zAxis;

        #region External Functions

        public Vector3 GetTileForwardDirection()
        {
            switch (tileRotationAxis)
            {
                case TileRotationAxis.xAxis:
                    return transform.right;

                case TileRotationAxis.yAxis:
                    return transform.up;

                case TileRotationAxis.zAxis:
                    return transform.forward;

                case TileRotationAxis.negXAxis:
                    return -transform.right;

                case TileRotationAxis.negYAxis:
                    return -transform.up;

                case TileRotationAxis.negZAxis:
                    return -transform.forward;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Enums

        public enum TileRotationAxis
        {
            xAxis,
            negXAxis,
            yAxis,
            negYAxis,
            zAxis,
            negZAxis
        }

        #endregion
    }
}