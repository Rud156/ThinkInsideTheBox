using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeAnim : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    // Start is called before the first frame update

    private void OnEnable()
    {
        FaceObject.OnLoaded += FadeOut;
    }

    private void OnDisable()
    {
        FaceObject.OnLoaded -= FadeOut;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            FadeOut();
        }
    }

    void FadeOut()
    {
        //Debug.Log("??????????");
        transition.SetTrigger("Start");
    }

    void FadeIn()
    {

    }
}
