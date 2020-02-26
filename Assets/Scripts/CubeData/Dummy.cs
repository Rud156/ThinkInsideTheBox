using System;
using System.Collections;
using UnityEngine;

namespace CubeData{
    public class Dummy : MonoBehaviour
    {
        public GameObject Projection;
        public LayerMask WalkableLayer;
        public CubeLayerMask tendingDirection = CubeLayerMask.Zero;
        private bool directionChanged = false;
        private CubeLayerMask gravityDirection = CubeLayerMask.down;
        private Vector3 m_destination;
        private float tolerance = 0.1f;

        #region Singleton
        public static Dummy Instance = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        private void Start()
        {
            StartCoroutine(MoveToCubie(tendingDirection));
        }

        private void Update()
        {
            
        }

        public IEnumerator MoveToCubie(CubeLayerMask i_direction)
        {
            CubeLayerMask pendingDirection;
            bool changed;
            (pendingDirection, changed) = GetCurrentCubie().GetMoveDirection(i_direction);

            if (pendingDirection == CubeLayerMask.Zero)
            {
                if (i_direction == tendingDirection)
                {
                    Debug.Log("Stop");
                    yield break;
                }
                if (!GetCurrentCubie().CanExit(tendingDirection))
                {
                    Debug.Log("Stop");
                    yield break;
                }
            }

            // Move action
            m_destination = GetNextPosition(pendingDirection);
            //if (pendingDirection == CubeLayerMask.up && !changed)
            //{
            //    transform.position = m_destination;
            //}
            //else
            {
                while (Vector3.Distance(m_destination, transform.position) > tolerance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, m_destination, Time.deltaTime);
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2,
                        out hit, WalkableLayer))
                    {
                        Projection.transform.position = hit.point;
                        Projection.transform.rotation = Quaternion.LookRotation(hit.normal);
                    }
                    else
                    {
                        Projection.transform.position = transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2;
                        Projection.transform.rotation = Quaternion.identity;
                    }
                    yield return null;
                }
                transform.position = m_destination;
                GetCurrentCubie().OnPlayerEnter(this);
                Debug.Log("Reach destination");
            }
            StartCoroutine(MoveToCubie(tendingDirection));
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