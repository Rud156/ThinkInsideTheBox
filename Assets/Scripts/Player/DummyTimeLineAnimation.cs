using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Player
{
    public class DummyTimeLineAnimation : MonoBehaviour
    {
        public Dummy playerController;
        public TimelineAsset jumpAsset;
        public TimelineAsset idleAsset;
        public PlayableDirector playerTimelineDirector;

        private PlayerAnimState m_playerAnimState;

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
            playerTimelineDirector.extrapolationMode = DirectorWrapMode.None;
            SetPlayerAnimState(PlayerAnimState.Jumping);
        }

        private void HandlePlayerMovementStopped()
        {
            if (m_playerAnimState == PlayerAnimState.Idle)
            {
                return;
            }

            playerTimelineDirector.Stop();
            playerTimelineDirector.Play(idleAsset);
            playerTimelineDirector.extrapolationMode = DirectorWrapMode.Loop;
            SetPlayerAnimState(PlayerAnimState.Idle);
        }

        private void SetPlayerAnimState(PlayerAnimState i_playerAnimState) => m_playerAnimState = i_playerAnimState;

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