using System;
using System.Collections;
using System.Collections.Generic;
using Player;
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

        public CubeLayerMask GetMoveDirection(CubeLayerMask i_direction)
        {
            // 1. Ground tile change direction
            CubeLayerMask pendingDirection;
            (pendingDirection) = GetGroundDirection(i_direction);
            if (pendingDirection == CubeLayerMask.Zero)
                return (CubeLayerMask.Zero);
            Assert.AreNotEqual(pendingDirection, CubeLayerMask.Zero);
            if (pendingDirection != CubeLayerMask.up &&
                pendingDirection != CubeLayerMask.down)
                Dummy.Instance.tendingDirection = pendingDirection;

            // If can't move to next cubie, change pending direction
            if (!CanMoveToNextCubie(pendingDirection))
            {
                if (pendingDirection != CubeLayerMask.down)
                {
                    Dummy.Instance.tendingDirection = -pendingDirection;
                    return (CubeLayerMask.Zero);
                }
                pendingDirection = i_direction;
            }

            // 2. Can exit current tile in that direction?
            if (!CanExit(pendingDirection))
            {
                Dummy.Instance.tendingDirection = -pendingDirection;
                return (CubeLayerMask.Zero);
            }

            // 3. Can enter next tile in that direction?
            CubieObject nextCubie;
            if (CubeWorld.TryGetNextCubie(Dummy.Instance.gameObject.transform.position, pendingDirection, out nextCubie))
            {
                if (!nextCubie.CanEnter(pendingDirection))
                {
                    if (pendingDirection != CubeLayerMask.down)
                    {
                        Dummy.Instance.tendingDirection = -pendingDirection;
                        return (CubeLayerMask.Zero);
                    }
                    return (i_direction);
                }
            }
            else
            {
                return pendingDirection;
            }
            
            return (pendingDirection);
        }

        public void OnPlayerEnter(Dummy dummy)
        {
            GetPlanimetricTile(CubeLayerMask.down).OnPlayerEnter(dummy);
        }

        public bool CanMoveToNextCubie(CubeLayerMask i_direction)
        {
            if (!IsInside(Dummy.Instance.gameObject)) return true;
            return CanExit(i_direction) && (GetNextCubie(i_direction) == null || GetNextCubie(i_direction).CanEnter(i_direction));
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
            if (!IsInside(Dummy.Instance.gameObject)) return true;
            // Plane check
            FaceObject overlappingTile = GetPlanimetricTile(i_direction);
            if (overlappingTile)
                return overlappingTile.GetMoveDirection(i_direction) != CubeLayerMask.Zero;
            return true;
        }

        public bool IsInside(GameObject i_gameObject)
        {
            return Vector3.Distance(i_gameObject.transform.position, transform.position) < CubeWorld.CUBIE_LENGTH/2;
        }

        public CubeLayerMask GetGroundDirection(CubeLayerMask i_direction)
        {
            FaceObject overlappingTile = GetGroundFace();
            // Plane check
            
            CubeLayerMask pendingDirection = CubeLayerMask.down;

            if (overlappingTile)
            {
                (pendingDirection) = overlappingTile.TryChangeDirection(i_direction);
            }
            //CubieObject belowCubie;
            //if (CubeWorld.TryGetNextCubie(transform.position, CubeLayerMask.down, out belowCubie))
            //    if (belowCubie.GetGroundDirection(i_direction) == CubeLayerMask.up && pendingDirection == CubeLayerMask.down)
            //        return (i_direction);
            if (!IsInside(Dummy.Instance.gameObject) &&
                pendingDirection == CubeLayerMask.down &&
                GetPlanimetricTile(CubeLayerMask.down).TryChangeDirection(i_direction) == CubeLayerMask.up)
                return i_direction;

            return (pendingDirection);
        }

        public FaceObject GetGroundFace()
        {
            if (IsInside(Dummy.Instance.gameObject))
                return GetPlanimetricTile(CubeLayerMask.down);
            else
                return GetPlanimetricTile(CubeLayerMask.up);
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