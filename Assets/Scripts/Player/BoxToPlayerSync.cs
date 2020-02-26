using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BoxToPlayerSync : MonoBehaviour
{


    public void sync(Transform playerTransform)
    {
        this.transform.rotation = playerTransform.transform.rotation;
        
    }

    private void OnTriggerEnter(Collider i_other)
    {
        if (i_other.transform.parent.CompareTag(TagManager.SideParent))
        {
            transform.SetParent(i_other.transform.parent);
        }
    }


    private void OnTriggerExit(Collider other)
    {
            transform.SetParent(null);
    }
}
