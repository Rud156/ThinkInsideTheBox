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
        public Dummy playerController;
        public DummyManualMovement playerManualMovement;

        [Header("Scene Switching")] public List<PlayerSpawn> playerSpawnData;
        public bool useFirstSpawnPoint = true;
        public int worldNumber = -1;
        public float sceneTransitionEndDelay = 1f;

        private Transform m_playerTransform;

        #region Unity Functions

        private void Start()
        {
            m_playerTransform = GameObject.FindGameObjectWithTag(TagManager.Player).transform;
            FaceObject.OnLoaded += UpdateSceneDataOnSwitch;

            StartCoroutine(StartPlayerSpawn());
        }

        private void OnDestroy() => FaceObject.OnLoaded -= UpdateSceneDataOnSwitch;

        #endregion

        #region External Functions

        public void CheckAndDisconnectSocket() => cubeInputController.CloseSocketConnection();

        #endregion

        #region Utility Functions

        private IEnumerator StartPlayerSpawn()
        {
            playerManualMovement.DisableMovement();
            PlayerSpawn playerSpawn = useFirstSpawnPoint ? playerSpawnData[0] : playerSpawnData.Find(_ => _.worldNumber == SceneData.LastWorldNumber);
            m_playerTransform.position = playerSpawn.initialSpawnPosition.position;

            yield return new WaitForSeconds(sceneTransitionEndDelay);

            playerController.ManuallyMoveTo(playerSpawn.initialPlayerMovementDirection);
            playerManualMovement.EnableMovement();
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