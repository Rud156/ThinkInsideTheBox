using UnityEngine;
using UnityEngine.UI;
using Utils;

public class InventorySystem : MonoBehaviour
{
    public Canvas GUI;

    [Header("Apple Collectible")] public int appleCount;
    public int maxAppleCount;

    [Header("Banana Collectible")] public int bananaCount;
    public int maxBananaCount;

    [Header("Cherry Collectible")] public int cherryCount;
    public int maxCherryCount;

    private bool _winnable;

    public bool Winnable
    {
        // get
        // {
        //     return this._winnable;
        // }

        // TODO: Put the actual value after levels are made
        get => true;
    }

    #region Unity Functions

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
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        appleCount = 0;
        bananaCount = 0;
        cherryCount = 0;
        _winnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (appleCount == maxAppleCount)
            _winnable = true;
    }

    #endregion

    #region External Functions

    public void ShowCollected()
    {
        Debug.Log("hitting");
        foreach (Transform child in GUI.transform)
        {
            if (child.gameObject.CompareTag(TagManager.Collected))
            {
                child.gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void MarkCollected(GameObject ToCollect)
    {
        if (ToCollect.gameObject.CompareTag(TagManager.AppleCollectible))
        {
            foreach (Transform child in GUI.transform)
            {
                if (appleCount < maxAppleCount && !child.gameObject.CompareTag(TagManager.Collected))
                {
                    appleCount++;
                    child.gameObject.tag = TagManager.Collected;
                    ShowCollected();
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
                    ShowCollected();
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
                    ShowCollected();
                    break;
                }
            }
        }
    }

    #endregion
}