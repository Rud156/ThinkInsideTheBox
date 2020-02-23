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
            CubeLayerMask pendingDirection = GetExitDirection(i_direction);
            
            CubieObject nextCubie;
            if (CubeWorld.TryGetNextCubie(transform.position, pendingDirection, out nextCubie))
                if (!nextCubie.CanEnter(pendingDirection))
                    return CubeLayerMask.Zero;

            return pendingDirection;
        }

        // Entering a cubie doesn't change the moving direction
        public bool CanEnter(CubeLayerMask i_direction)
        {
            bool result = true;
            if (VolumetricTile)
                result &= VolumetricTile.GetMoveDirection(i_direction) == i_direction;
            TileObject groundTile = GetPlanimetricTile(CubeLayerMask.down);
            if (groundTile)
                result &= groundTile.GetMoveDirection(i_direction) == i_direction;
            return result;
        }

        public CubeLayerMask GetExitDirection(CubeLayerMask i_direction)
        {
            if (VolumetricTile)
                return VolumetricTile.GetMoveDirection(i_direction);
            TileObject groundTile = GetPlanimetricTile(CubeLayerMask.down);
            if (groundTile)
                return groundTile.GetMoveDirection(i_direction);
            return i_direction;
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