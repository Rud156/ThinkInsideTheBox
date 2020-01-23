using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Scenes.Main
{
    public class MainSceneController : MonoBehaviour
    {
        public float waitBeforeLevelLoading = 1;

        #region Unity Functions

        private void Update()
        {
            if (Input.GetKeyDown(ControlConstants.Restart))
            {
                ReloadCurrentLevel(true);
            }
            else if (Input.GetKeyDown(ControlConstants.Quit))
            {
                Application.Quit();
            }
        }

        #endregion

        #region External Functions

        #region Level Loading

        public void ReloadCurrentLevel(bool instantReload = false)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(LoadLevel(buildIndex, instantReload));
        }

        public void LoadNextLevel(int index) => StartCoroutine(LoadLevel(index));

        #endregion

        #endregion

        #region Utility Functions

        private IEnumerator LoadLevel(int index, bool instantLoad = false)
        {
            if (instantLoad)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(waitBeforeLevelLoading);
            }

            SceneManager.LoadScene(index);
        }

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