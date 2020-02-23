using System;
using System.Collections;
using UnityEngine;

namespace CubeData{
    public class Dummy : MonoBehaviour
    {
        public CubeLayerMask startDirection = CubeLayerMask.down;
        private CubeLayerMask gravityDirection = CubeLayerMask.down;
        private Vector3 m_destination;
        private float tolerance = 0.1f;

        private void Start()
        {
            StartCoroutine(Move(startDirection));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Move(CubeLayerMask.up));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(Move(CubeLayerMask.down));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Move(CubeLayerMask.back));
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Move(CubeLayerMask.forward));
            }
        }

        public IEnumerator Move(CubeLayerMask i_direction)
        {
            yield return StartCoroutine(MoveToCubie(i_direction));
        }

        public IEnumerator MoveToCubie(CubeLayerMask i_direction)
        {
            CubeLayerMask pendingDirection = GetMoveDirection(i_direction);
            if (pendingDirection == CubeLayerMask.Zero)
            {
                Debug.Log("Reach destination");
            }
            else
            {
                // Move action
                m_destination = GetNextPosition(pendingDirection);
                //Debug.Log("Moving from" + transform.position + " to" + m_destination);
                //while (Vector3.Distance(m_destination, transform.position) > tolerance)
                //{
                //    transform.position = Vector3.MoveTowards(transform.position, m_destination, Time.deltaTime);
                //    yield return null;
                //}
                //Debug.Log("Reach cubie");
                if (pendingDirection == i_direction)
                {
                    while (Vector3.Distance(m_destination, transform.position) > tolerance)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, m_destination, Time.deltaTime);
                        yield return null;
                    }
                    transform.position = m_destination;
                    StartCoroutine(MoveToCubie(gravityDirection));
                }
                else
                {
                    transform.position = m_destination;
                    StartCoroutine(MoveToCubie(i_direction));
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
            if (CubeWorld.TryGetNextCubie(transform.position, i_direction, out cubie))
                return cubie.transform.position;
            return GetCurrentCubie().transform.position + i_direction.ToVector3() * CubeWorld.CUBIE_LENGTH;
        }

        private CubieObject GetCurrentCubie()
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.forward * CubeWorld.CUBIE_LENGTH / 2, transform.position, out hit, CubeWorld.CUBIE_LAYER_MASK))
            {
                return hit.transform.GetComponent<CubieObject>();
            }
            throw new Exception("Player is not inside any TileObject");
        }
    }
}