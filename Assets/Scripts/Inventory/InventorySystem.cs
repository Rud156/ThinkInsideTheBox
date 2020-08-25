using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    private int _currentCount = 0;
    private int _totalCount = 0;

    public GameObject currentUI;
    public GameObject totalUI;
    public GameObject UI;
    public GameObject FadeOut;

    private bool[] collectList;

    public static InventorySystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        _totalCount = SceneManager.sceneCountInBuildSettings - 3; // TODO: Maybe set it directly...
        totalUI.GetComponent<Text>().text = _totalCount.ToString();
        collectList = new bool[_totalCount];
        for (int i = 0; i < _totalCount; i++)
            collectList[i] = false;
    }

    #region External Functions

    public bool CheckStat(int levelId) => collectList[levelId];

    public void Initialize()
    {
        for (int i = 0; i < _totalCount; i++)
        {
            collectList[i] = false;
        }

        _currentCount = 0;
        currentUI.GetComponent<Text>().text = _currentCount.ToString();
    }

    IEnumerator WinGame()
    {
        FadeOut.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        UI.SetActive(false);
        yield return new WaitForSeconds(.8f);

        FadeOut.GetComponent<Animator>().SetBool("fade", false);
    }

    public void MarkCollected(int currentlevel)
    {
        if (collectList[currentlevel] == false)
        {
        }

        _currentCount++;
        currentUI.GetComponent<Text>().text = _currentCount.ToString();
        collectList[currentlevel] = true;
        if (_currentCount == _totalCount)
            StartCoroutine(WinGame());
    }

    public bool[] GetCollectionList() => collectList;

    #endregion
}