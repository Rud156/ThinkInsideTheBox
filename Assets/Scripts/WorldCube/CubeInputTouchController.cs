using System;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeInputTouchController : MonoBehaviour
    {
        [Header("RayCast")] public Camera mainCamera;
        public LayerMask layerMask;

        [Header("Direction Distance")] public float minDistanceBeforeChecking;

        private CubeControllerV2 m_cubeController;
        private CubeRotationHandler m_cubeRotationHandler;

        private SideTouched m_currentSideTouched;
        private MovementDirection m_movementDirection;

        private Vector2 m_lastMousePosition;

        #region Unity Functions

        private void Start()
        {
            m_cubeController = GetComponent<CubeControllerV2>();
            m_cubeRotationHandler = GetComponent<CubeRotationHandler>();
        }

        private void Update()
        {
            UpdateSideDrag();

            // Only perform the RayCast when the mouse button is down
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    m_lastMousePosition = Input.mousePosition;
                    HandleSideTouched(hit.transform.tag);
                }
                else
                {
                    Debug.LogError("Nothing was hit");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleSideUnTouched();
            }
        }

        #endregion

        #region Utility Functions

        private void UpdateSideDrag()
        {
            if (m_currentSideTouched == SideTouched.None)
            {
                return;
            }

            Vector2 currentMousePosition = Input.mousePosition;

            if (m_movementDirection == MovementDirection.None)
            {
                float xDiff = Mathf.Abs(currentMousePosition.x - m_lastMousePosition.x);
                float yDiff = Mathf.Abs(currentMousePosition.y - m_lastMousePosition.y);

                if (xDiff > minDistanceBeforeChecking)
                {
                    m_lastMousePosition = currentMousePosition;
                    SetMovementDirection(MovementDirection.Horizontal);
                }
                else if (yDiff > minDistanceBeforeChecking)
                {
                    m_lastMousePosition = currentMousePosition;
                    SetMovementDirection(MovementDirection.Vertical);
                }
            }
            else
            {
                float xDiff = Mathf.Abs(currentMousePosition.x - m_lastMousePosition.x);
                float yDiff = Mathf.Abs(currentMousePosition.y - m_lastMousePosition.y);

                switch (m_movementDirection)
                {
                    case MovementDirection.Horizontal:
                    {
                        if (xDiff > minDistanceBeforeChecking)
                        {
                            int direction = (int) Mathf.Sign(m_lastMousePosition.x - currentMousePosition.x);
                            m_lastMousePosition = currentMousePosition;

                            UpdateCubeRotation(direction);
                        }
                    }
                        break;

                    case MovementDirection.Vertical:
                    {
                        if (yDiff > minDistanceBeforeChecking)
                        {
                            int direction = (int) Mathf.Sign(m_lastMousePosition.y - currentMousePosition.y);
                            m_lastMousePosition = currentMousePosition;

                            UpdateCubeRotation(direction);
                        }
                    }
                        break;

                    case MovementDirection.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateCubeRotation(int i_direction)
        {
            switch (m_currentSideTouched)
            {
                case SideTouched.None:
                    break;

                case SideTouched.Top:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), i_direction);
                    }
                }
                    break;

                case SideTouched.Bottom:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), -i_direction);
                    }
                }
                    break;

                case SideTouched.Left:
                {
                    if (m_movementDirection == MovementDirection.Vertical)
                    {
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Left), i_direction);
                    }
                }
                    break;

                case SideTouched.Right:
                {
                    if (m_movementDirection == MovementDirection.Vertical)
                    {
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Right), -i_direction);
                    }
                }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleSideTouched(string i_touchedSide)
        {
            switch (i_touchedSide)
            {
                case TagManager.TopTouchTag:
                    SetSideTouched(SideTouched.Top);
                    break;

                case TagManager.BottomTouchTag:
                    SetSideTouched(SideTouched.Bottom);
                    break;

                case TagManager.LeftTouchTag:
                    SetSideTouched(SideTouched.Left);
                    break;

                case TagManager.RightTouchTag:
                    SetSideTouched(SideTouched.Right);
                    break;

                default:
                    // Do nothing here...
                    break;
            }
        }

        private void HandleSideUnTouched()
        {
            if (m_currentSideTouched == SideTouched.None)
            {
                return;
            }

            // Also do other things...
            SetSideTouched(SideTouched.None);
            SetMovementDirection(MovementDirection.None);
        }

        private void SetSideTouched(SideTouched i_sideTouched) => m_currentSideTouched = i_sideTouched;

        private void SetMovementDirection(MovementDirection i_movementDirection) => m_movementDirection = i_movementDirection;

        #endregion

        #region Enums

        private enum SideTouched
        {
            None,
            Top,
            Bottom,
            Left,
            Right
        }

        private enum MovementDirection
        {
            None,
            Horizontal,
            Vertical
        }

        #endregion
    }
}