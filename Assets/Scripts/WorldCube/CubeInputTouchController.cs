using System;
using CustomCamera;
using UnityEngine;
using UnityScript.Macros;
using Utils;
using Player;

namespace WorldCube
{
    public class CubeInputTouchController : MonoBehaviour
    {
        // Note: The Front and Back are flipped to make it same as that of the Player

        [Header("RayCast")] public Camera mainCamera;
        public float maxRayCastDistance = 100.0f;
        public LayerMask layerMask;
        public LayerMask materialLayerMask;

        [Header("Direction Distance")] public float minDistanceBeforeResponse;

        [Header("Camera")] public CameraController cameraController;
        public float cameraRotationMultiplier;

        [Header("Player")] public Dummy m_playerController;

        private CubeControllerV2 m_cubeController;
        private CubeRotationHandler m_cubeRotationHandler;
        private CubeSideTransparencyControl m_cubeSideTransparencyControl;

        private Side m_CurrentSide;
        private MovementDirection m_movementDirection;
        private CubeSideTouchIndicator m_currentCubeSideTouchIndicator;

        private Vector2 m_lastMousePosition;
        private Transform m_lastHoveredSide;

        private bool m_controllerActive = true;

        #region Unity Functions

        private void Start()
        {
            m_cubeController = GetComponent<CubeControllerV2>();
            m_cubeRotationHandler = GetComponent<CubeRotationHandler>();
            m_cubeSideTransparencyControl = GetComponent<CubeSideTransparencyControl>();
        }

        private void Update()
        {
            if (m_playerController.IsPlayerMoving() || !m_controllerActive)
            {
                return;
            }

            UpdateSideDrag();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            bool didRayCastHit = Physics.Raycast(ray, out RaycastHit hitMaterial, maxRayCastDistance, materialLayerMask);
            CheckAndUpdateSideTransparency(hitMaterial, didRayCastHit);

            // Only perform the RayCast when the mouse button is down
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out RaycastHit hit, maxRayCastDistance, layerMask))
                {
                    m_lastMousePosition = Input.mousePosition;
                    HandleSideTouched(hit.transform.gameObject);
                }
                else
                {
                    SetSideTouched(Side.NoneOnlyClicked);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleSideUnTouched();
            }
        }

        #endregion

        #region External Functions

        public void ForceStopController()
        {
            m_controllerActive = false;
        }

        public void ForceStartController()
        {
            m_controllerActive = true;
        }

        #endregion

        #region Utility Functions

        private void CheckAndUpdateSideTransparency(RaycastHit i_hit, bool i_didRayCastHit)
        {
            if (i_didRayCastHit)
            {
                Transform target = i_hit.transform;
                if (target != m_lastHoveredSide)
                {
                    if (m_lastHoveredSide != null)
                    {
                        m_cubeSideTransparencyControl.MakeSideTransparent(m_lastHoveredSide);
                    }

                    m_lastHoveredSide = target;

                    if (m_lastHoveredSide != null)
                    {
                        m_cubeSideTransparencyControl.MakeSideVisible(m_lastHoveredSide);
                    }
                }
            }
            else
            {
                if (m_lastHoveredSide != null)
                {
                    m_cubeSideTransparencyControl.MakeSideTransparent(m_lastHoveredSide);
                }

                m_lastHoveredSide = null;
            }
        }

        private void UpdateSideDrag()
        {
            if (m_CurrentSide == Side.None)
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

            if (m_CurrentSide == Side.NoneOnlyClicked)
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
                                int direction = (int)Mathf.Sign(m_lastMousePosition.x - currentMousePosition.x);
                                m_lastMousePosition = currentMousePosition;

                                UpdateCubeRotation(direction, m_movementDirection);
                            }
                        }
                        break;

                    case MovementDirection.Vertical:
                        {
                            if (yDiff > minDistanceBeforeResponse)
                            {
                                int direction = (int)Mathf.Sign(m_lastMousePosition.y - currentMousePosition.y);
                                m_lastMousePosition = currentMousePosition;

                                UpdateCubeRotation(direction, m_movementDirection);
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

        private void UpdateCubeRotation(int i_direction, MovementDirection i_movementDirection)
        {
            if (m_currentCubeSideTouchIndicator == null)
            {
                Debug.LogError("Invalid Controller");
                return;
            }

            switch (i_movementDirection)
            {
                case MovementDirection.None:
                    // Don't do anything here...
                    break;

                case MovementDirection.Horizontal:
                case MovementDirection.Vertical:
                    {
                        Side side = m_currentCubeSideTouchIndicator.GetSideForMovement(i_movementDirection);
                        int directionMultiplier = m_currentCubeSideTouchIndicator.GetDirectionMultiplierForMovement(i_movementDirection);

                        switch (side)
                        {
                            case Side.None:
                            case Side.SideSelected:
                            case Side.NoneOnlyClicked:
                                // Don't do anything here...
                                break;

                            case Side.Top:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Up, i_direction * directionMultiplier);
                                break;

                            case Side.Bottom:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Down, i_direction * directionMultiplier);
                                break;

                            case Side.Left:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Left, i_direction * directionMultiplier);
                                break;

                            case Side.Right:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Right, i_direction * directionMultiplier);
                                break;

                            case Side.Front:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Back, i_direction * directionMultiplier);
                                break;

                            case Side.Back:
                                m_cubeController.CheckAndUpdateRotation(CubeLayerMaskV2.Forward, i_direction * directionMultiplier);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_movementDirection), i_movementDirection, null);
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

        private void HandleSideTouched(GameObject side)
        {
            CubeSideTouchIndicator cubeSideTouchIndicator = side.GetComponent<CubeSideTouchIndicator>();
            if (!cubeSideTouchIndicator)
            {
                return;
            }

            m_currentCubeSideTouchIndicator = cubeSideTouchIndicator;
            SetSideTouched(Side.SideSelected);
        }

        private void HandleSideUnTouched()
        {
            if (m_CurrentSide == Side.None)
            {
                return;
            }

            m_currentCubeSideTouchIndicator = null;

            SetSideTouched(Side.None);
            SetMovementDirection(MovementDirection.None);
        }

        private void SetSideTouched(Side i_Side) => m_CurrentSide = i_Side;

        private void SetMovementDirection(MovementDirection i_movementDirection) => m_movementDirection = i_movementDirection;

        #endregion

        #region Enums

        public enum Side
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back,

            SideSelected,
            NoneOnlyClicked, // Probably a very bad name
        }

        public enum MovementDirection
        {
            None,
            Horizontal,
            Vertical
        }

        #endregion
    }
}