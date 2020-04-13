using UnityEngine;
using UnityEngine.UI;
using Utils;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    /*[Header("Apple Collectible")] public int appleCount;
    public int maxAppleCount;

    [Header("Banana Collectible")] public int bananaCount;
    public int maxBananaCount;

    [Header("Cherry Collectible")] public int cherryCount;
    public int maxCherryCount;*/

    private bool _winnable = false;
    private int _currentCount = 0;
    private int _totalCount = 0;

    public GameObject currentUI;
    public GameObject totalUI;

    public bool Winnable
    {
        // get
        // {
        //     return this._winnable;
        // }

        // TODO: Put the actual value after levels are made
        get => true;
    }

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

    /*public void ShowCollected()
    {
        foreach (Transform child in GUI.transform)
        {
            if (child.gameObject.CompareTag(TagManager.Collected))
            {

                
               child.GetChild(_totalCount).gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    } */

    public void MarkCollected(int currentlevel)
    {
        if (collectList[currentlevel] == false)
        {
            _currentCount++;
            currentUI.GetComponent<Text>().text = _currentCount.ToString();
            collectList[currentlevel] = true;
            if (_currentCount == _totalCount)
                _winnable = true;
        } 

        /*if (ToCollect.gameObject.CompareTag(TagManager.AppleCollectible))
        {
            appleCount++;
            _totalCount++;
            //ShowCollected();
            foreach (Transform child in GUI.transform)
            {
                if (appleCount < maxAppleCount && !child.gameObject.CompareTag(TagManager.Collected))
                {

                    break;
                }
            }
        }
        else if(ToCollect.gameObject.CompareTag(TagManager.BananaCollectible))
        {
            foreach (Transform child in GUI.transform)
            {
                if (bananaCount < maxBananaCount && !child.gameObject.CompareTag(TagManager.Collected))
                {
                    bananaCount++;
                    child.gameObject.tag = TagManager.Collected;
                    //ShowCollected();
                    break;
                }
            }
        }
        else if (ToCollect.gameObject.CompareTag(TagManager.CherryCollectible))
        {
            foreach (Transform child in GUI.transform)
            {
                if (cherryCount < maxCherryCount && !child.gameObject.CompareTag(TagManager.Collected))
                {
                    cherryCount++;
                    child.gameObject.tag = TagManager.Collected;
                    //ShowCollected();
                    break;
                }
            }
        }
        */
    }

    #endregion
}