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
            CubeLayerMask pendingDirection;
            bool changed;
            (pendingDirection, changed) = GetGroundDirection(i_direction);
            Assert.AreNotEqual(pendingDirection, CubeLayerMask.Zero);
            if (pendingDirection != CubeLayerMask.up && changed)
                Dummy.Instance.tendingDirection = pendingDirection;

            CubieObject nextCubie;
            if (CubeWorld.TryGetNextCubie(transform.position, pendingDirection, out nextCubie))
            {
                if (!nextCubie.CanEnter(pendingDirection))
                {
                    if (!changed)
                        // Direction can be changed
                        Dummy.Instance.tendingDirection = -pendingDirection;
                    return (CubeLayerMask.Zero, !changed);
                }
            }
            else
            {
                throw new Exception("Not found next cubie");
                //Dummy.Instance.tendingDirection = -pendingDirection;
                //return CubeLayerMask.Zero;
            }
            return (pendingDirection, changed);
            //return keepDirection ? i_direction : exitDirection;
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

        public (CubeLayerMask, bool) GetGroundDirection(CubeLayerMask i_direction)
        {
            // Plane check
            FaceObject overlappingTile = GetPlanimetricTile(CubeLayerMask.down);
            if (overlappingTile)
                return overlappingTile.TryChangeDirection(i_direction);
            return (i_direction, false);
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
    }

}