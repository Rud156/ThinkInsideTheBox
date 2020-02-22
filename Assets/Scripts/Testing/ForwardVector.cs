using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForwardVector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.forward);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
            Debug.Log(transform.forward);
    }
}
