using CubeData;
using UnityEngine;

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

        public Dummy playerController;

        private CubeLayerMask m_playerMovementDirection;

        #region Unity Functions

        private void Start() => m_playerMovementDirection = new CubeLayerMask(0, 0, 0);

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


            StartCoroutine(playerController.MoveToCubie(m_playerMovementDirection));
        }

        #endregion
    }
}