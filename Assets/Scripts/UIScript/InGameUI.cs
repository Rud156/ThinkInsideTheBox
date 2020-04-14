using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{

    public static InGameUI Instance = null;

    public GameObject FadeOut;
    public GameObject UI;

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

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnMenu()
    {
        StartCoroutine(transition());
    }

    IEnumerator transition()
    {
        FadeOut.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(0);
        UI.SetActive(false);
        yield return new WaitForSeconds(.8f);
        FadeOut.GetComponent<Animator>().SetBool("fade", false);

    }
}
