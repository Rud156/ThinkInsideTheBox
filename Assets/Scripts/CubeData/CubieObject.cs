using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WorldCube;

namespace CubeData
{
    public class CubieObject : MonoBehaviour
    {

        private bool m_hasParent;
        private CubeLayerMaskV2 m_currentLayerMask;
        private Transform m_parentTransform;
        private Transform m_originalParent;

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
                    if (pendingDirection != CubeLayerMask.down)
                    {
                        Dummy.Instance.tendingDirection = -pendingDirection;
                        return (CubeLayerMask.Zero, true);
                    }
                    return (i_direction, !force);
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

        public bool CanMoveToNextCubie(CubeLayerMask i_direction)
        {
            return CanExit(i_direction) && GetNextCubie(i_direction).CanEnter(i_direction);
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
            CubeLayerMask pendingDirection = CubeLayerMask.down;
            bool dirctionChanged = false;

            if (overlappingTile)
            {
                (pendingDirection, dirctionChanged) = overlappingTile.TryChangeDirection(i_direction);
            }

            CubieObject belowCubie;
            if (CubeWorld.TryGetNextCubie(transform.position, CubeLayerMask.down, out belowCubie))
                if (belowCubie.GetGroundDirection(i_direction).Item1 == CubeLayerMask.up && pendingDirection == CubeLayerMask.down)
                    return (i_direction, false);

            return (pendingDirection, dirctionChanged); //(CubeLayerMask.down, true);
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

        private void Start() => m_originalParent = transform.parent;

        public void SetParent(CubeLayerMaskV2 iCubeLayerMask, Transform i_parentTransform)
        {
            if (m_hasParent)
            {
                throw new Exception("Trying to access cube that already has a parent");
            }

            m_hasParent = true;
            m_currentLayerMask = iCubeLayerMask;
            m_parentTransform = i_parentTransform;

            transform.SetParent(i_parentTransform);
        }

        public void ReleaseParent(CubeLayerMaskV2 i_cubeLayerMask)
        {
            if (i_cubeLayerMask != m_currentLayerMask)
            {
                throw new Exception("Invalid layer is trying to release");
            }

            m_hasParent = false;
            m_currentLayerMask = null;
            m_parentTransform = null;

            transform.SetParent(m_originalParent);
        }

        public bool HasParent => m_hasParent;

    }

}