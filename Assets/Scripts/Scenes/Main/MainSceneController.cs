using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Main
{
    public class MainSceneController : MonoBehaviour
    {
        public float waitBeforeLevelLoading = 1;

        #region External Functions

        #region Level Loading

        public void ReloadCurrentLevel()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(LoadLevel(buildIndex));
        }

        public void LoadNextLevel(int index)
        {
            StartCoroutine(LoadLevel(index));
        }

        #endregion

        #endregion

        #region Utility Functions

        private IEnumerator LoadLevel(int index)
        {
            yield return new WaitForSeconds(waitBeforeLevelLoading);
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex);
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