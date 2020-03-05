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
        if (Input.GetKeyDown(KeyCode.T))
        {
            float xPos = Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
            float yPos = Mathf.Sin(-transform.eulerAngles.x * Mathf.Deg2Rad);
            float zPos = Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad) * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);

            print(xPos + ", " + yPos + ", " + zPos + ", " + transform.forward);
        }
    }
}