using System;
using System.Collections;
using UnityEngine;

namespace CubeData{
    public class Dummy : MonoBehaviour
    {
        private enum PlayerState
        {
            Moving, CanMove, Suspending, Stuck, Ending
        }

        public GameObject Projection;
        public LayerMask WalkableLayer;
        public CubeLayerMask tendingDirection = CubeLayerMask.Zero;
        private bool directionChanged = false;
        private CubeLayerMask gravityDirection = CubeLayerMask.down;
        private Vector3 m_destination;
        private float tolerance = 0.1f;
        private PlayerState m_playerState = PlayerState.Stuck;
        private float m_MoveSpeed = 1f;

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

        public bool IsPlayerMoving() => m_playerState == PlayerState.Moving;

        public void PreventPlayerMovement() => m_playerState = PlayerState.Suspending;
        public void AllowPlayerMovement()
        {
            if (m_playerState == PlayerState.Moving) return;
            m_playerState = PlayerState.CanMove;
        }

        private void Start()
        {
            StartCoroutine(MoveToCubie(tendingDirection));
        }

        private void FixedUpdate()
        {
            Debug.DrawLine(transform.position, transform.position + 
                tendingDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2, Color.cyan);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & CubeWorld.CUBIE_LAYER_MASK) != 0)
                transform.SetParent(other.transform);
        }

        public void RotateTendingDirection()
        {
            CubeLayerMask direction = new CubeLayerMask(transform.forward);
            if (direction.y != 0) return;
            tendingDirection = direction;
        }

        public IEnumerator MoveToCubie(CubeLayerMask i_direction)
        {
            if (m_playerState == PlayerState.Moving || m_playerState == PlayerState.Ending)
                yield break;
            CubeLayerMask pendingDirection;
            bool changed;
            (pendingDirection, changed) = GetCurrentCubie().GetMoveDirection(i_direction);
            m_playerState = PlayerState.Moving;

            if (pendingDirection == CubeLayerMask.Zero)
            {
                if (i_direction == tendingDirection || 
                    (!GetCurrentCubie().CanMoveToNextCubie(tendingDirection) && 
                    !GetCurrentCubie().CanMoveToNextCubie(-tendingDirection)))
                {
                    Debug.Log("Stop");
                    m_playerState = PlayerState.Stuck;
                    yield break;
                }
            }

            // Move action
            m_destination = GetNextPosition(pendingDirection);
            if (pendingDirection.y == 0)
            {
                transform.LookAt(m_destination);
            }
            if (pendingDirection != CubeLayerMask.up)
            {
                m_MoveSpeed = pendingDirection == CubeLayerMask.down ? 5f : 1f;
                while (Vector3.Distance(m_destination, transform.position) > tolerance)
                {
                    SetPlayerPosition(Vector3.MoveTowards(transform.position, m_destination, Time.deltaTime * m_MoveSpeed), pendingDirection);
                    yield return null;
                }
            }
            SetPlayerPosition(m_destination, pendingDirection);
            GetCurrentCubie().OnPlayerEnter(this);
            Debug.Log("Reach destination");

            float waitSecond = 2;
            if (IsFailling())
                waitSecond = 0.01f;
            m_playerState = PlayerState.CanMove;
            yield return new WaitForSeconds(waitSecond);
            yield return new WaitUntil(() => m_playerState == PlayerState.CanMove);
            
            StartCoroutine(MoveToCubie(tendingDirection));
        }

        private bool IsFailling()
        {
            return GetCurrentCubie().GetPlanimetricTile(gravityDirection).GetMoveDirection(gravityDirection) == gravityDirection;
        }

        private void SetPlayerPosition(Vector3 i_Position, CubeLayerMask i_PendingDir)
        {
            transform.position = i_Position; RaycastHit hit;
            if (Physics.Linecast(transform.position - gravityDirection.ToVector3() * 1.1f, transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH,
                out hit, WalkableLayer))
            {
                Projection.transform.position = hit.point;
                Projection.transform.rotation = Quaternion.LookRotation(
                    Quaternion.AngleAxis(Vector3.Cross(i_PendingDir.ToVector3(), hit.normal).x > 0 ? -90f : 90f, Vector3.right) * hit.normal);
            }
            else
            {
                Projection.transform.position = transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2;
                if (i_PendingDir.y == 0)
                    Projection.transform.localRotation = Quaternion.identity;
            }
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