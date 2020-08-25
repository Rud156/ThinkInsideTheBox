using System.Collections;
using System.Collections.Generic;
using CubeData;
using Player;
using UnityEngine;
using Utils;
using WorldCube;

namespace Scenes.Main
{
    public class MainSceneController : MonoBehaviour
    {
        [Header("Components")] public CubeInputController cubeInputController;
        public CubeInputTouchController touchController;
        public Dummy playerController;
        public DummyManualMovement playerManualMovement;

        [Header("Scene Switching")] public List<PlayerSpawn> playerSpawnData;
        public bool isInsideWorld = true;
        public int worldNumber = -1;
        public float sceneTransitionEndDelay = 1f;

        [Header("Custom Player Movement")] public bool useCustomPlayerMovement;
        public DummyTimeLineAnimation playerAnimation;
        public float playerLerpSpeed;
        public List<Transform> playerMovementPositions;
        public CubeLayerMask finalMovementDirection;

        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private float m_lerpAmount;
        private int m_currentPositionIndex;
        private bool m_isLerpActive;

        private Transform m_playerTransform;

        #region Unity Functions

        private void Start()
        {
            FaceObject.OnLoaded += UpdateSceneDataOnSwitch;

            GameObject player = GameObject.FindGameObjectWithTag(TagManager.Player);
            m_playerTransform = player.transform;

            StartCoroutine(StartPlayerSpawn());
        }

        private void OnDestroy() => FaceObject.OnLoaded -= UpdateSceneDataOnSwitch;

        private void Update()
        {
            if (!m_isLerpActive)
            {
                return;
            }

            m_lerpAmount += playerLerpSpeed * Time.deltaTime;
            if (m_lerpAmount >= 0.97f)
            {
                m_playerTransform.position = m_targetPosition;
                Debug.Log("Lerp Ended. Checking and Switching");

                playerAnimation.PlayIdleAnim(); // Reset Anim State
                m_lerpAmount = 0;
                m_currentPositionIndex += 1;

                if (m_currentPositionIndex >= playerMovementPositions.Count)
                {
                    m_isLerpActive = false;

                    playerController.ManuallyMoveTo(finalMovementDirection);
                    playerManualMovement.EnableMovement();
                    playerController.transform.SetParent(playerController.GetCurrentCubie()?.transform);
                    
                    touchController.ForceStartController();
                    Debug.Log("Player Movement Complete");
                }
                else
                {
                    m_startPosition = m_playerTransform.position;
                    m_targetPosition = playerMovementPositions[m_currentPositionIndex].position;
                    playerAnimation.PlayJumpAnim();
                }
            }
            else
            {
                m_playerTransform.position = Vector3.Lerp(m_startPosition, m_targetPosition, m_lerpAmount);
            }
        }

        #endregion

        #region External Functions

        public void CheckAndDisconnectSocket() => cubeInputController.CloseSocketConnection();

        #endregion

        #region Utility Functions

        private IEnumerator StartPlayerSpawn()
        {
            playerManualMovement.DisableMovement();
            PlayerSpawn playerSpawn = isInsideWorld ? playerSpawnData[0] : playerSpawnData.Find(_ => _.worldNumber == SceneData.LastWorldNumber);
            m_playerTransform.position = playerSpawn.initialSpawnPosition.position;

            yield return new WaitForSeconds(sceneTransitionEndDelay);

            if (useCustomPlayerMovement)
            {
                touchController.ForceStopController();
                m_currentPositionIndex = 0;

                m_startPosition = m_playerTransform.position;
                m_targetPosition = playerMovementPositions[m_currentPositionIndex].position;
                m_lerpAmount = 0;
                m_isLerpActive = true;
                playerAnimation.PlayJumpAnim();

                Debug.Log("Started Custom Player Movement");
            }
            else
            {
                playerController.ManuallyMoveTo(playerSpawn.initialPlayerMovementDirection);
                playerManualMovement.EnableMovement();
            }
        }

        private void UpdateSceneDataOnSwitch() => SceneData.LastWorldNumber = worldNumber;

        #endregion

        #region Singleton

        private static MainSceneController _instance;

        public static MainSceneController Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region Structs

    [System.Serializable]
    public struct PlayerSpawn
    {
        public Transform initialSpawnPosition;
        public CubeLayerMask initialPlayerMovementDirection;
        public int worldNumber;
    }

    #endregion
}