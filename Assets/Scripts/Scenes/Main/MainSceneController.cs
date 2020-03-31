using UnityEngine;
using WorldCube;

namespace Scenes.Main
{
    public class MainSceneController : MonoBehaviour
    {
        public CubeInputController cubeInputController;

        #region External Functions

        public void CheckAndDisconnectSocket() => cubeInputController.CloseSocketConnection();

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
}