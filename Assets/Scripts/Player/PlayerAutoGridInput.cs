using System;
using UnityEngine;

namespace Player
{
    public class PlayerAutoGridInput : MonoBehaviour
    {
        public float stopTimeBetweenPositions;
        public Direction startDirection;

        private float m_currentTimer;
        private bool m_isMovementActive;

        private Direction m_currentDirection;

        #region Unity Functions

        private void Start()
        {
            m_currentDirection = startDirection;
        }

        private void Update()
        {
        }

        #endregion

        #region External Functions

        public void StartPlayerMovement()
        {
            m_isMovementActive = true;
        }

        public void StopPlayerMovement()
        {
            m_isMovementActive = false;
        }

        #endregion

        #region Utility Functions

        private void UpdateMovementTimer()
        {
            if (!m_isMovementActive)
            {
                return;
            }

            m_currentTimer -= Time.deltaTime;
            if (m_currentTimer <= 0)
            {
                FindNextMovementSpot();

                m_currentTimer = stopTimeBetweenPositions;
            }
        }

        private void FindNextMovementSpot()
        {
        }

        #endregion

        #region Enums

        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right
        }

        #endregion
    }
}