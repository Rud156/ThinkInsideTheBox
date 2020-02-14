using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public Canvas GUI;

    [Header("Box Collectible")]
    public int boxCount;
    public int maxBoxCount;

    [Header("Circle Collectible")]
    public int circleCount;
    public int maxCircleCount;

    private bool _winnable;
    public bool Winnable
    {
        get
        {
            return this._winnable;
        }
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
        boxCount = 0;
        maxBoxCount = 3;
        _winnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (boxCount == maxBoxCount)
            _winnable = true;
    }

    #endregion

    #region External Functions
    public void ShowCollected()
    {
        Debug.Log("hitting");
        foreach(Transform child in GUI.transform)
        {
            if (child.gameObject.CompareTag(TagManager.Collected))
            {
                child.gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }
    public void MarkCollected(GameObject ToCollect)
    {
        if (ToCollect.gameObject.CompareTag(TagManager.BoxCollectible))
        {
            foreach (Transform child in GUI.transform)
            {
                if (boxCount < maxBoxCount && !child.gameObject.CompareTag(TagManager.Collected))
                {
                    boxCount++;
                    child.gameObject.tag = TagManager.Collected;
                    ShowCollected();
                    break;
                }
            }
        }
        else
        {
            foreach (Transform child in GUI.transform)
            {
                if (circleCount < maxCircleCount && !child.gameObject.CompareTag(TagManager.Collected))
                {
                    circleCount++;
                    child.gameObject.tag = TagManager.Collected;
                    ShowCollected();
                    break;
                }
            }
        }
    }
    #endregion
}
