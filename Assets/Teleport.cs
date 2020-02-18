using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using WorldCube;

public class Teleport : MonoBehaviour
{
    public GameObject destination;
    public PlayerGridController playerGridController;
    public CubeControllerV2 cubeController;

    private Canvas fadeCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        fadeCanvas = GameObject.FindGameObjectWithTag("FadeCanvas").GetComponent<Canvas>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destination == null)
            Debug.LogError("Destination not assigned!");
        else
        {
            playerGridController.GetComponent<Rigidbody>().isKinematic = true;
            playerGridController.PreventPlayerMovement();
            Vector3 targetCubeCenter = destination.transform.position + new Vector3(0, 1, 0); //playerGridController.GetComponent<PlayerGridInput>().FindPositionOnFace(destination.transform.position);
            playerGridController.transform.position = targetCubeCenter;
            playerGridController.SetPlayerInControl();
            StartCoroutine("FadeIO");

            
            //playerGridController.AllowPlayerMovement();
            //cubeController.EndWorldFlip();

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            playerGridController.AllowPlayerMovement();
    }

    IEnumerator FadeIO()
    {
        FadeOutScreen();

        yield return new WaitForSeconds(1.5f);
        
        Debug.Log("???");
        cubeController.EndWorldFlip();
        FadeInScreen();
        
    }

    private void FadeOutScreen()
    {
        //Debug.Log("Screen fading");
        fadeCanvas.GetComponent<Animator>().SetBool("Fading", true);
        //fadeImg.CrossFadeAlpha(1, 0.5f, false);
    }
    private void FadeInScreen()
    {
        fadeCanvas.GetComponent<Animator>().SetBool("Fading", false);
        //fadeImg.CrossFadeAlpha(0, 2f, false);
    }


}
