using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubieObject : MonoBehaviour
    {
        public LayerMask PlaneLayerMask;
        public TileObject VolumetricTile;
        public bool keepDirection;
        public CubeLayerMask exitDirection = CubeLayerMask.Zero;

        public CubeLayerMask GetMoveDirection(CubeLayerMask i_direction)
        {
            if (VolumetricTile)
                return VolumetricTile.GetMoveDirection(i_direction);
            TileObject groundTile = GetPlanimetricTile(CubeLayerMask.down);
            if (groundTile)
                return groundTile.GetMoveDirection(i_direction);
            return i_direction;
            //return keepDirection ? i_direction : exitDirection;
        }

        public TileObject GetPlanimetricTile(CubeLayerMask i_direction)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, i_direction.ToVector3(), out hit, Mathf.Infinity, PlaneLayerMask))
            {
                Debug.DrawRay(transform.position, i_direction.ToVector3() * hit.distance, Color.blue, 3f);
                return hit.transform.GetComponentInChildren<TileObject>();
            }
            throw new System.Exception("No plane found");
        }
    }

}