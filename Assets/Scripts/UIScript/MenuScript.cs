using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{


    #region External Functions

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
    

    public void SelectLevel(int i_Index)
    {
        SceneManager.LoadScene(i_Index);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
