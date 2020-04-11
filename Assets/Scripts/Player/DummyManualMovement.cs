using CubeData;
using UnityEngine;
using Utils;
using WorldCube;

namespace Player
{
    public class DummyManualMovement : MonoBehaviour
    {
        private const string LeftStr = "Left";
        private const string RightStr = "Right";
        private const string TopStr = "Top";
        private const string BottomStr = "Bottom";
        private const string FrontStr = "Front";
        private const string BackStr = "Back";

        public CubeControllerV2 cubeControllerV2;
        public CubeRotationHandler cubeRotationHandler;
        public Dummy playerController;

        private CubeLayerMask m_playerMovementDirection;
        private bool m_isMovementEnabled;

        #region Unity Functions

        private void Start()
        {
            m_isMovementEnabled = false;
            m_playerMovementDirection = new CubeLayerMask(0, 0, 0);
        }

        private void Update()
        {
            if (!cubeControllerV2.IsMovementAllowed || !m_isMovementEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(ControlConstants.Left) || Input.GetKeyDown(ControlConstants.AltLeft))
            {
                m_playerMovementDirection = cubeRotationHandler.GetCubeLayerMask(cubeRotationHandler.Left);
                playerController.ManuallyMoveTo(m_playerMovementDirection);
            }
            else if (Input.GetKeyDown(ControlConstants.Right) || Input.GetKeyDown(ControlConstants.AltRight))
            {
                m_playerMovementDirection = cubeRotationHandler.GetCubeLayerMask(cubeRotationHandler.Right);
                playerController.ManuallyMoveTo(m_playerMovementDirection);
            }
            else if (Input.GetKeyDown(ControlConstants.Forward) || Input.GetKeyDown(ControlConstants.AltForward))
            {
                m_playerMovementDirection = cubeRotationHandler.GetCubeLayerMask(cubeRotationHandler.Forward);
                playerController.ManuallyMoveTo(m_playerMovementDirection);
            }
            else if (Input.GetKeyDown(ControlConstants.Back) || Input.GetKeyDown(ControlConstants.AltBack))
            {
                m_playerMovementDirection = cubeRotationHandler.GetCubeLayerMask(cubeRotationHandler.Back);
                playerController.ManuallyMoveTo(m_playerMovementDirection);
            }
        }

        #endregion

        #region External Functions

        public void UpdateMovementDirection(string i_movementDirection)
        {
            Debug.Log($"Movement Direction: {i_movementDirection}");

            switch (i_movementDirection)
            {
                case LeftStr:
                    m_playerMovementDirection = CubeLayerMask.left;
                    break;

                case RightStr:
                    m_playerMovementDirection = CubeLayerMask.right;
                    break;

                case TopStr:
                    // This is because Top is not a valid movement direction
                    return;

                case BottomStr:
                    // This is because Bottom is not a valid movement direction
                    return;

                case FrontStr:
                    m_playerMovementDirection = CubeLayerMask.forward;
                    break;

                case BackStr:
                    m_playerMovementDirection = CubeLayerMask.back;
                    break;
            }


            playerController.ManuallyMoveTo(m_playerMovementDirection);
        }

        public void EnableMovement() => m_isMovementEnabled = true;

        public void DisableMovement() => m_isMovementEnabled = false;

        #endregion
    }
}