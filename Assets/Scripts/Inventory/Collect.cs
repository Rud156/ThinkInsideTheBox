using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Collect : MonoBehaviour
{

    #region Unity Functions

    private void OnTriggerEnter(Collider i_other)
    {

        if (i_other.gameObject.CompareTag(TagManager.Player))
        {
            InventorySystem.Instance.MarkCollected(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Utility Functions


    #endregion
}
