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
        public bool keepDirection;
        public CubeLayerMask exitDirection = CubeLayerMask.Zero;

        public (CubeLayerMask, bool) GetMoveDirection(CubeLayerMask i_direction)
        {
            // 1. Ground tile change direction
            CubeLayerMask pendingDirection;
            bool force;
            (pendingDirection, force) = GetGroundDirection(i_direction);
            if (pendingDirection == CubeLayerMask.Zero)
                return (CubeLayerMask.Zero, false);
            Assert.AreNotEqual(pendingDirection, CubeLayerMask.Zero);
            if (pendingDirection != CubeLayerMask.up &&
                pendingDirection != CubeLayerMask.down &&
                force)
                Dummy.Instance.tendingDirection = pendingDirection;

            // 2. Can exit current tile in that direction?
            if (!CanExit(pendingDirection))
            {
                if (!force)
                {
                    Dummy.Instance.tendingDirection = -pendingDirection;
                    return (CubeLayerMask.Zero, true);
                }
                return (CubeLayerMask.Zero, false);
            }

            // 3. Can enter next tile in that direction?
            CubieObject nextCubie;
            if (CubeWorld.TryGetNextCubie(transform.position, pendingDirection, out nextCubie))
            {
                if (!nextCubie.CanEnter(pendingDirection))
                {
                    if (!force)
                    {
                        Dummy.Instance.tendingDirection = -pendingDirection;
                        return (CubeLayerMask.Zero, true);
                    }
                    return (CubeLayerMask.Zero, !force);
                }
            }
            else
            {
                throw new Exception(pendingDirection + " Not found next cubie");
                //Dummy.Instance.tendingDirection = -pendingDirection;
                //return CubeLayerMask.Zero;
            }
            return (pendingDirection, force);
            //return keepDirection ? i_direction : exitDirection;
        }

        public void OnPlayerEnter(Dummy dummy)
        {
            GetPlanimetricTile(CubeLayerMask.down).OnPlayerEnter(dummy);
        }

        // Entering a cubie doesn't change the moving direction
        public bool CanEnter(CubeLayerMask i_direction)
        {
            // Plane check
            FaceObject overlappingTile = GetPlanimetricTile(-i_direction);
            if (overlappingTile)
                return overlappingTile.GetMoveDirection(i_direction) != CubeLayerMask.Zero;
            return true;
        }

        public bool CanExit(CubeLayerMask i_direction)
        {
            // Plane check
            FaceObject overlappingTile = GetPlanimetricTile(i_direction);
            if (overlappingTile)
                return overlappingTile.GetMoveDirection(i_direction) != CubeLayerMask.Zero;
            return true;
        }

        public (CubeLayerMask, bool) GetGroundDirection(CubeLayerMask i_direction)
        {
            // Plane check
            FaceObject overlappingTile = GetPlanimetricTile(CubeLayerMask.down);
            if (overlappingTile)
                return overlappingTile.TryChangeDirection(i_direction);
            CubieObject belowCubie;
            if (CubeWorld.TryGetNextCubie(transform.position, CubeLayerMask.down, out belowCubie))
                if (belowCubie.GetGroundDirection(i_direction).Item1 == CubeLayerMask.up)
                    return (i_direction, false);
            return (CubeLayerMask.down, true);
        }

        public FaceObject GetPlanimetricTile(CubeLayerMask i_direction)
        {
            Assert.AreNotEqual(i_direction, CubeLayerMask.Zero);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, i_direction.ToVector3(), out hit, Mathf.Infinity, PlaneLayerMask))
            {
                Debug.DrawRay(transform.position, i_direction.ToVector3() * hit.distance, Color.blue, 3f);
                return hit.transform.GetComponentInChildren<FaceObject>();
            }
            throw new Exception(gameObject.name + i_direction.ToString() + "No plane found");
        }

        public CubieObject GetNextCubie(CubeLayerMask i_direction)
        {
            CubieObject next;
            CubeWorld.TryGetNextCubie(transform.position, i_direction, out next);
            return next;
        }

    }

}