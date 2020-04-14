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

    #region Singleton

    public static InventorySystem Instance = null;
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

        _totalCount = SceneManager.sceneCountInBuildSettings - 2;
        totalUI.GetComponent<Text>().text = _totalCount.ToString();
        collectList = new bool[_totalCount];
        for (int i = 0; i < _totalCount; i++)
            collectList[i] = false;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    #region External Functions

    public bool CheckStat(int levelId)
    {
        return collectList[levelId];
    }

    public void Initialize()
    {
        for (int i = 0; i < _totalCount; i++)
            collectList[i] = false;
    }

    IEnumerator winGame()
    {
        FadeOut.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(0);
        UI.SetActive(false);
        yield return new WaitForSeconds(.8f);
        FadeOut.GetComponent<Animator>().SetBool("fade", false);
    }

    public void MarkCollected(int currentlevel)
    {
        if (collectList[currentlevel] == false)
        {
            _currentCount++;
            currentUI.GetComponent<Text>().text = _currentCount.ToString();
            collectList[currentlevel] = true;
            if (_currentCount == _totalCount)
                StartCoroutine(winGame());
        } 
    }

    #endregion
}