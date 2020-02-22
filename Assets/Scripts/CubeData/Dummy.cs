using System;
using System.Collections;
using UnityEngine;

namespace CubeData{
    public class Dummy : MonoBehaviour
    {
        public LayerMask CubieLayerMask;
        public float CubieLength = 1f;
        public CubeLayerMask startDirection = CubeLayerMask.down;
        private Stack directions = new Stack();
        private CubeLayerMask gravityDirection = CubeLayerMask.down;

        private void Start()
        {
            Move(startDirection);
        }

        public void Move(CubeLayerMask i_direction)
        {
            directions.Push(i_direction);
            while (directions.Count > 0)
            {
                CubeLayerMask pendingDirection = GetMoveDirection((CubeLayerMask)directions.Peek());
                if (pendingDirection == CubeLayerMask.Zero)
                {
                    Debug.Log("Reach destination");
                    directions.Clear();
                    return;
                }
                else if (pendingDirection == (CubeLayerMask)directions.Peek())
                {
                    // Move action
                    transform.position = GetNextPosition((CubeLayerMask)directions.Pop());
                    if (directions.Count == 0)
                        directions.Push(gravityDirection);
                }
                else
                {
                    directions.Push(pendingDirection);
                }
            }
        }

        public CubeLayerMask GetMoveDirection(CubeLayerMask i_direction)
        {
            return GetCurrentCubie() ? GetCurrentCubie().GetMoveDirection(i_direction) : i_direction;
        }

        public Vector3 GetNextPosition(CubeLayerMask i_direction)
        {
            CubieObject cubie;
            if (TryGetNextCubie(i_direction, out cubie))
                return cubie.transform.position;
            return GetCurrentCubie().transform.position + i_direction.ToVector3() * CubieLength;
        }

        private CubieObject GetCurrentCubie()
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.forward * CubieLength / 2, transform.position, out hit, CubieLayerMask))
            {
                return hit.transform.GetComponent<CubieObject>();
            }
            return null;
            throw new Exception("Player is not inside any TileObject");
        }

        private bool TryGetNextCubie(CubeLayerMask i_direction, out CubieObject o_cubie)
        {
            RaycastHit hit;
            o_cubie = null;
            if (Physics.Raycast(transform.position, i_direction.ToVector3(), out hit, Mathf.Infinity, CubieLayerMask))
            {
                Debug.DrawRay(transform.position, i_direction.ToVector3() * CubieLength, Color.red, 3f, false);
                o_cubie = hit.transform.GetComponent<CubieObject>();
                return true;
            }
            return false;
        }
    }
}