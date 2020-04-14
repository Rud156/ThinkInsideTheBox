//#define OUTSIDE
using System;
using System.Collections;
using CubeData;
using UnityEngine;

namespace Player
{
    public class Dummy : MonoBehaviour
    {
        private enum PlayerState
        {
            Moving,
            CanMove,
            Suspending,
            Stuck,
            Ending
        }

        public GameObject Projection;
        public LayerMask WalkableLayer;
        public float WalkSpeed = 1f;
        public CubeLayerMask tendingDirection = CubeLayerMask.Zero;
        public bool AutoMovement = true;
        public CubeLayerMask pendingDirection;
        private bool directionChanged = false;
        private CubeLayerMask gravityDirection = CubeLayerMask.down;
        public GameObject m_movingTarget;
        private GameObject m_lastStableFloor;
        private Vector3 m_lastStablePos;
        private Vector3 m_movingPos;
        private Quaternion m_destRot = Quaternion.identity;
        private float tolerance = 0.1f;
        private PlayerState m_playerState = PlayerState.Stuck;
        private bool m_stopped = false;
        private float m_MoveSpeed = 1f;

        public delegate void PlayerMovementActivated();
        public delegate void PlayerMovementStopped();
        public PlayerMovementActivated OnPlayerMovementActivated;
        public PlayerMovementStopped OnPlayerMovementStopped;

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
        public bool IsPlayerStuck() => m_stopped;

        public void PreventPlayerMovement()
        {
            m_playerState = PlayerState.Suspending;
        }

        public void AllowPlayerMovement()
        {
            if (m_playerState == PlayerState.Moving) return;
            m_playerState = PlayerState.CanMove;
        }

        private void Start()
        {
            if (AutoMovement)
                StartCoroutine(MoveToCubie(tendingDirection));
            else
                m_stopped = true;
            SetProjectionPosition(tendingDirection);
        }

        private void FixedUpdate()
        {
            Debug.DrawLine(transform.position, transform.position +
                                               tendingDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2, Color.cyan);
        }

        public void RotateTendingDirection()
        {
            CubeLayerMask direction = new CubeLayerMask(transform.forward);
            if (direction.y != 0) return;
            tendingDirection = AutoMovement ? direction : CubeLayerMask.down;
        }

        public void ManuallyMoveTo(CubeLayerMask i_direction)
        {
            tendingDirection = i_direction;
            StartCoroutine(MoveToCubie(i_direction));
        }

        public IEnumerator MoveToCubie(CubeLayerMask i_direction)
        {
            if (m_playerState == PlayerState.Moving || m_playerState == PlayerState.Ending)
                yield break;
            if (GetCurrentCubie())
            {
                pendingDirection = GetCurrentCubie().GetMoveDirection(i_direction);
            }
            else
            {
                pendingDirection = gravityDirection;
            }
            m_playerState = PlayerState.Moving;

            if (pendingDirection == CubeLayerMask.Zero)
            {
                if (i_direction == tendingDirection ||
                    (!GetCurrentCubie().CanMoveToNextCubie(tendingDirection) &&
                     !GetCurrentCubie().CanMoveToNextCubie(-tendingDirection)))
                {
                    OnPlayerMovementStopped?.Invoke();
                    
                    Debug.Log("Stop");
                    SetProjectionPosition(pendingDirection);
                    m_stopped = true;
                    m_playerState = PlayerState.Stuck;
                    yield break;
                }
            }

            OnPlayerMovementActivated?.Invoke();

            // Move action
            m_movingTarget = GetNextTarget(pendingDirection);
            m_movingPos = GetNextPos(pendingDirection);
            if (pendingDirection.y == 0)
            {
                Quaternion q = Projection.transform.rotation;
                if (m_movingTarget)
                    transform.LookAt(m_movingTarget.transform.position);
                else
                    transform.LookAt(m_movingPos);
                Projection.transform.rotation = q;
            }
            if (pendingDirection.y == 0)
            {
                m_destRot = Quaternion.LookRotation(pendingDirection.ToVector3());
                StartCoroutine(RotateTo(m_destRot));
            }
            
            if (pendingDirection != CubeLayerMask.up)
            {
                m_MoveSpeed = pendingDirection == CubeLayerMask.down ? 5 * WalkSpeed : WalkSpeed;
                if (m_movingTarget)
                    while (Vector3.Distance(m_movingTarget.transform.position, transform.position) > tolerance && m_playerState == PlayerState.Moving)
                    {
                        SetPlayerPosition(Vector3.MoveTowards(transform.position, m_movingTarget.transform.position, Time.deltaTime * m_MoveSpeed), pendingDirection);
                        yield return null;
                    }
                else
                    while (Vector3.Distance(m_movingPos, transform.position) > tolerance && m_playerState == PlayerState.Moving)
                    {
                        SetPlayerPosition(Vector3.MoveTowards(transform.position, m_movingPos, Time.deltaTime * m_MoveSpeed), pendingDirection);
                        yield return null;
                    }

            }
            if(m_playerState == PlayerState.Stuck)
            {
                yield break;
            }

            m_stopped = false;
            if (m_movingTarget)
                SetPlayerPosition(m_movingTarget.transform.position, pendingDirection);
            else
                SetPlayerPosition(m_movingPos, pendingDirection);
            m_movingTarget = null;
            if(GetCurrentCubie() && GetCurrentCubie().IsInside(gameObject))
            {
                GetCurrentCubie()?.OnPlayerEnter(this);
            }

            transform.SetParent(GetCurrentCubie()?.transform);
            if (GetCurrentCubie())
            {
                if (GetCurrentCubie().IsInside(gameObject))
                    m_lastStableFloor = GetCurrentCubie().gameObject;
                else
                {
                    m_lastStableFloor = null;
                    m_lastStablePos = m_movingPos;
                }
            }
            OnPlayerMovementStopped?.Invoke();
            Debug.Log("Reach destination");

            StartCoroutine(RotateTo(m_destRot));
            float waitSecond = 2;
            bool fallingBeforeRotation = IsFalling();
            if (fallingBeforeRotation)
                waitSecond = 0.01f;
            m_playerState = PlayerState.CanMove;
            yield return new WaitForSeconds(waitSecond);
            yield return new WaitUntil(() => m_playerState == PlayerState.CanMove);
            bool fallingAfterRotation = IsFalling();
            if (!fallingBeforeRotation && fallingAfterRotation)
                yield return new WaitForSeconds(1f);

            if (IsFalling() && pendingDirection != CubeLayerMask.up)
                tendingDirection = gravityDirection;
            if (AutoMovement || IsFalling() || pendingDirection == CubeLayerMask.up)
                StartCoroutine(MoveToCubie(tendingDirection));
            else
            {
                Debug.Log("Stop");
                SetProjectionPosition(pendingDirection);
                m_stopped = true;
                m_playerState = PlayerState.Stuck;
                yield break;
            }
        }

        public bool IsFalling()
        {
            return GetCurrentCubie() == null || GetCurrentCubie().GetGroundFace().GetMoveDirection(gravityDirection) == gravityDirection;
            //return GetCurrentCubie().GetPlanimetricTile(gravityDirection).GetMoveDirection(gravityDirection) == gravityDirection;
        }

        private void SetProjectionPosition(CubeLayerMask i_PendingDir)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position - gravityDirection.ToVector3() * 0.6f,
                transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH,
                out hit, WalkableLayer))
            {
                Projection.transform.position = hit.point;
            }
            else
            {
                Projection.transform.position = transform.position + gravityDirection.ToVector3() * CubeWorld.CUBIE_LENGTH / 2 + Vector3.up * 0.25f;
            }
        }

        public IEnumerator RotateTo(Quaternion i_to)
        {
            float time = 0;
            Quaternion q = Projection.transform.rotation;
            while (Quaternion.Angle(Projection.transform.rotation, i_to) > 0.5f)
            {
                Projection.transform.rotation = Quaternion.Slerp(q, i_to, time);
                time += Time.deltaTime;
                yield return null;
            }

            Projection.transform.rotation = m_destRot;
            Debug.Log("Rotation complete");
        }

        private void SetPlayerPosition(Vector3 i_Position, CubeLayerMask i_PendingDir)
        {
            transform.position = i_Position;
            SetProjectionPosition(i_PendingDir);
        }

        public GameObject GetNextTarget(CubeLayerMask i_direction)
        {
            CubieObject cubie;
            if (CubeWorld.TryGetNextCubie(transform.position, i_direction, out cubie))
                return cubie.gameObject;
            return null;
        }

        public Vector3 GetNextPos(CubeLayerMask i_direction)
        {
            CubieObject cubie;
            if (CubeWorld.TryGetNextCubie(transform.position, i_direction, out cubie))
                return cubie.gameObject.transform.position;
            return transform.position + i_direction.ToVector3() * CubeWorld.CUBIE_LENGTH;
        }

        public CubieObject GetCurrentCubie()
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.forward * CubeWorld.CUBIE_LENGTH / 2, transform.position, out hit, CubeWorld.CUBIE_LAYER_MASK))
            {
                return hit.transform.GetComponent<CubieObject>();
            }
            if (Physics.Linecast(transform.position, transform.position + Vector3.down * CubeWorld.CUBIE_LENGTH, out hit, CubeWorld.CUBIE_LAYER_MASK))
            {
                return hit.transform.GetComponent<CubieObject>();
            }
            return null;
        }

        public void ResetToLastFloor()
        {
            gameObject.transform.position = m_lastStableFloor ? m_lastStableFloor.transform.position : m_lastStablePos;
            if (!AutoMovement)
            {
                tendingDirection = CubeLayerMask.Zero;
                m_stopped = true;
                m_playerState = PlayerState.Stuck;
                m_movingTarget = m_lastStableFloor?.gameObject;
                m_movingPos = m_lastStablePos;
            }
                
        }
    }
}