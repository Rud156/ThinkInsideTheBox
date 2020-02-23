using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
            {
                Debug.Log(nextCubie.transform.name);
                if (!nextCubie.CanEnter(pendingDirection))
                    return CubeLayerMask.Zero;
            }
            return pendingDirection;
        }

        // Entering a cubie doesn't change the moving direction
        public bool CanEnter(CubeLayerMask i_direction)
        {
            // Volume check
            bool result = true;
            if (VolumetricTile)
                result &= VolumetricTile.GetMoveDirection(i_direction) == i_direction;

            // Plane check
            TileObject overlappingTile = GetPlanimetricTile(-i_direction);
            if (overlappingTile)
                result &= overlappingTile.GetMoveDirection(i_direction) == i_direction;

            return result;
        }

        public CubeLayerMask GetExitDirection(CubeLayerMask i_direction)
        {
            // Volume check
            CubeLayerMask pendingDirection = i_direction;
            if (VolumetricTile)
                pendingDirection = VolumetricTile.GetMoveDirection(i_direction);
            if (pendingDirection == CubeLayerMask.Zero)
                return pendingDirection;

            // Plane check
            TileObject overlappingTile = GetPlanimetricTile(pendingDirection);
            if (overlappingTile)
                pendingDirection = overlappingTile.GetMoveDirection(pendingDirection);

            return pendingDirection;
        }

        public TileObject GetPlanimetricTile(CubeLayerMask i_direction)
        {
            Assert.AreNotEqual(i_direction, CubeLayerMask.Zero);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, i_direction.ToVector3(), out hit, Mathf.Infinity, PlaneLayerMask))
            {
                Debug.DrawRay(transform.position, i_direction.ToVector3() * hit.distance, Color.blue, 3f);
                return hit.transform.GetComponentInChildren<TileObject>();
            }
            throw new Exception(gameObject.name + i_direction.ToString() + "No plane found");
        }
    }

}