using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Collect : MonoBehaviour
{
    public InventorySystem myInventory;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider i_other)
    {

        if (i_other.gameObject.CompareTag(TagManager.Player))
        {
            myInventory.MarkCollected(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Utility Functions


    #endregion
}
