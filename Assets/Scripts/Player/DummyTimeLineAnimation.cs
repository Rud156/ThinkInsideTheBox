using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace Player
{
    public class DummyTimeLineAnimation : MonoBehaviour
    {
        public Dummy playerController;
        public TimelineAsset jumpAsset;
        public List<IdleAnimAsset> idleAssets;
        public PlayableDirector playerTimelineDirector;

        private PlayerAnimState m_playerAnimState;
        private float m_currentTimer;

        #region Unity Functions

        private void Start()
        {
            playerController.OnPlayerMovementActivated += HandlePlayerMovementActivated;
            playerController.OnPlayerMovementStopped += HandlePlayerMovementStopped;

            SetPlayerAnimState(PlayerAnimState.None);
            HandlePlayerMovementStopped();
        }

        private void OnDestroy()
        {
            playerController.OnPlayerMovementActivated -= HandlePlayerMovementActivated;
            playerController.OnPlayerMovementStopped -= HandlePlayerMovementStopped;
        }

        private void Update()
        {
            switch (m_playerAnimState)
            {
                case PlayerAnimState.None:
                    break;

                case PlayerAnimState.Jumping:
                    break;

                case PlayerAnimState.Idle:
                {
                    m_currentTimer -= Time.deltaTime;
                    if (m_currentTimer <= 0)
                    {
                        PlayRandomIdleAnimResetTimer();
                    }
                }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void PlayJumpAnim() => HandlePlayerMovementActivated();

        public void PlayIdleAnim() => HandlePlayerMovementStopped();

        #endregion

        #region Utility Functions

        private void HandlePlayerMovementActivated()
        {
            if (m_playerAnimState == PlayerAnimState.Jumping)
            {
                return;
            }

            playerTimelineDirector.Stop();
            playerTimelineDirector.Play(jumpAsset);
            SetPlayerAnimState(PlayerAnimState.Jumping);
        }

        private void HandlePlayerMovementStopped()
        {
            if (m_playerAnimState == PlayerAnimState.Idle)
            {
                return;
            }

            PlayRandomIdleAnimResetTimer();
            SetPlayerAnimState(PlayerAnimState.Idle);
        }

        private void PlayRandomIdleAnimResetTimer()
        {
            int randomIndex = Random.Range(0, 1000) % idleAssets.Count;
            IdleAnimAsset idleAnimAsset = idleAssets[randomIndex];

            m_currentTimer = idleAnimAsset.idleRuntime;
            playerTimelineDirector.Stop();
            playerTimelineDirector.Play(idleAnimAsset.idleTimelineAsset);
        }

        private void SetPlayerAnimState(PlayerAnimState i_playerAnimState) => m_playerAnimState = i_playerAnimState;

        #endregion

        #region Structs

        [System.Serializable]
        public struct IdleAnimAsset
        {
            public float idleRuntime;
            public TimelineAsset idleTimelineAsset;
        }

        #endregion

        #region Enums

        private enum PlayerAnimState
        {
            None,
            Jumping,
            Idle
        }

        #endregion
    }
}