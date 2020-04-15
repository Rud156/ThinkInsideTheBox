using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{


    #region External Functions

    public void NewGame()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.Initialize();
    }
    

    public void SelectLevel(int i_Index)
    {
        SceneManager.LoadScene(i_Index);
        if (InGameUI.Instance != null)
            InGameUI.Instance.transform.Find("UI").gameObject.SetActive(true);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ToGym()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
        if (InGameUI.Instance != null)
            InGameUI.Instance.transform.Find("UI").gameObject.SetActive(true);
    }
    #endregion
}
