using System;
using CustomCamera;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeInputTouchController : MonoBehaviour
    {
        // Note: The Front and Back are flipped to make it same as that of the Player

        [Header("RayCast")] public Camera mainCamera;
        public float maxRayCastDistance = 100.0f;
        public LayerMask layerMask;

        [Header("Direction Distance")] public float minDistanceBeforeResponse;

        [Header("Camera")] public CameraController cameraController;
        public float cameraRotationMultiplier;

        private CubeControllerV2 m_cubeController;
        private CubeRotationHandler m_cubeRotationHandler;

        private SideTouched m_currentSideTouched;
        private MovementDirection m_movementDirection;

        private Vector2 m_lastMousePosition;

        private CubeSideTransparencyControl m_cubeSideTransparencyControl;

        #region Unity Functions

        private void Start()
        {
            m_cubeController = GetComponent<CubeControllerV2>();
            m_cubeRotationHandler = GetComponent<CubeRotationHandler>();
        }

        private void Update()
        {
            UpdateSideDrag();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            bool didRayCastHit = Physics.Raycast(ray, out RaycastHit hit, maxRayCastDistance, layerMask);
            CheckAndUpdateSideTransparency(hit, didRayCastHit);

            // Only perform the RayCast when the mouse button is down
            if (Input.GetMouseButtonDown(0))
            {
                if (didRayCastHit)
                {
                    m_lastMousePosition = Input.mousePosition;
                    HandleSideTouched(hit.transform.tag);
                }
                else
                {
                    SetSideTouched(SideTouched.NoneOnlyClicked);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleSideUnTouched();
            }
        }

        #endregion

        #region Utility Functions

        private void CheckAndUpdateSideTransparency(RaycastHit i_hit, bool i_success)
        {
            if (!i_success)
            {
                if (m_cubeSideTransparencyControl != null)
                {
                    m_cubeSideTransparencyControl.MakeSideTransparent();
                }

                m_cubeSideTransparencyControl = null;
                return;
            }

            // This can be assumed as the RayCast is only going to hit the TouchDetector Layer
            // This is still super inefficient. A better way will be to pre cache the components and get them from a list...
            CubeSideTransparencyControl cubeSideTransparencyControl = i_hit.transform.GetComponent<CubeSideTransparencyControl>();
            if (cubeSideTransparencyControl != m_cubeSideTransparencyControl)
            {
                if (m_cubeSideTransparencyControl != null)
                {
                    m_cubeSideTransparencyControl.MakeSideTransparent();
                }

                m_cubeSideTransparencyControl = cubeSideTransparencyControl;

                if (m_cubeSideTransparencyControl != null)
                {
                    m_cubeSideTransparencyControl.MakeSideVisible();
                }
            }
        }

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

                if (xDiff > minDistanceBeforeResponse)
                {
                    m_lastMousePosition = currentMousePosition;
                    SetMovementDirection(MovementDirection.Horizontal);
                }
                else if (yDiff > minDistanceBeforeResponse)
                {
                    m_lastMousePosition = currentMousePosition;
                    SetMovementDirection(MovementDirection.Vertical);
                }
            }

            if (m_currentSideTouched == SideTouched.NoneOnlyClicked)
            {
                UpdateCameraDrag();
            }
            else
            {
                float xDiff = Mathf.Abs(currentMousePosition.x - m_lastMousePosition.x);
                float yDiff = Mathf.Abs(currentMousePosition.y - m_lastMousePosition.y);

                switch (m_movementDirection)
                {
                    case MovementDirection.Horizontal:
                    {
                        if (xDiff > minDistanceBeforeResponse)
                        {
                            int direction = (int) Mathf.Sign(m_lastMousePosition.x - currentMousePosition.x);
                            m_lastMousePosition = currentMousePosition;

                            UpdateCubeRotation(direction);
                        }
                    }
                        break;

                    case MovementDirection.Vertical:
                    {
                        if (yDiff > minDistanceBeforeResponse)
                        {
                            int direction = (int) Mathf.Sign(m_lastMousePosition.y - currentMousePosition.y);
                            m_lastMousePosition = currentMousePosition;

                            UpdateCubeRotation(direction);
                        }
                    }
                        break;

                    case MovementDirection.None:
                        // Don't do anything here...
                        break;

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
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Top), i_direction);
                    }
                }
                    break;

                case SideTouched.Bottom:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Bottom), -i_direction);
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

                case SideTouched.Front:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Back), -i_direction);
                    }
                }
                    break;

                case SideTouched.Back:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Forward), i_direction);
                    }
                }
                    break;

                case SideTouched.TopLeft:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Top);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Top), i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Left);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Left), i_direction);
                    }
                }
                    break;

                case SideTouched.TopRight:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Top);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Top), i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Right);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Right), -i_direction);
                    }
                }
                    break;

                case SideTouched.BottomLeft:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Bottom);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Bottom), -i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Left);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Left), i_direction);
                    }
                }
                    break;

                case SideTouched.BottomRight:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Bottom);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(CubeRotationHandler.FaceDirection.Bottom), -i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Right);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Right), -i_direction);
                    }
                }
                    break;

                case SideTouched.FrontLeft:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Front);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Back), -i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Left);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Left), i_direction);
                    }
                }
                    break;

                case SideTouched.FrontRight:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Front);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Back), -i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Right);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Right), -i_direction);
                    }
                }
                    break;

                case SideTouched.BackLeft:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Back);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Forward), i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Left);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Left), i_direction);
                    }
                }
                    break;

                case SideTouched.BackRight:
                {
                    if (m_movementDirection == MovementDirection.Horizontal)
                    {
                        SetSideTouched(SideTouched.Back);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Forward), i_direction);
                    }
                    else if (m_movementDirection == MovementDirection.Vertical)
                    {
                        SetSideTouched(SideTouched.Right);
                        m_cubeController.CheckAndUpdateRotation(m_cubeRotationHandler.GetCubeLayerMaskV2(m_cubeRotationHandler.Right), -i_direction);
                    }
                }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCameraDrag()
        {
            if (m_movementDirection != MovementDirection.Horizontal)
            {
                return;
            }

            Vector2 currentMousePosition = Input.mousePosition;
            float xDiff = Mathf.Abs(currentMousePosition.x - m_lastMousePosition.x);

            if (xDiff > minDistanceBeforeResponse)
            {
                xDiff *= Mathf.Sign(m_lastMousePosition.x - currentMousePosition.x);
                m_lastMousePosition = currentMousePosition;

                cameraController.IncrementManualCameraRotation(xDiff * cameraRotationMultiplier);
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

                case TagManager.FrontTouchTag:
                    SetSideTouched(SideTouched.Front);
                    break;

                case TagManager.BackTouchTag:
                    SetSideTouched(SideTouched.Back);
                    break;

                case TagManager.TopLeftTouchTag:
                    SetSideTouched(SideTouched.TopLeft);
                    break;

                case TagManager.TopRightTouchTag:
                    SetSideTouched(SideTouched.TopRight);
                    break;

                case TagManager.BottomLeftTouchTag:
                    SetSideTouched(SideTouched.BottomLeft);
                    break;

                case TagManager.BottomRightTouchTag:
                    SetSideTouched(SideTouched.BottomRight);
                    break;

                case TagManager.FrontLeftTouchTag:
                    SetSideTouched(SideTouched.FrontLeft);
                    break;

                case TagManager.FrontRightTouchTag:
                    SetSideTouched(SideTouched.FrontRight);
                    break;

                case TagManager.BackLeftTouchTag:
                    SetSideTouched(SideTouched.BackLeft);
                    break;

                case TagManager.BackRightTouchTag:
                    SetSideTouched(SideTouched.BackRight);
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
            Right,
            Front,
            Back,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            FrontLeft,
            FrontRight,
            BackLeft,
            BackRight,

            NoneOnlyClicked, // Probably a very bad name
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