using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dummy>())
        {
            other.GetComponent<Dummy>().ResetToLastFloor();
        }
    }
}
