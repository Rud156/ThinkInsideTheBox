using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;

public class Collect : MonoBehaviour
{
    private int currentlevel;

    private void Start()
    {
        currentlevel = SceneManager.GetActiveScene().buildIndex - 1;
        if (InventorySystem.Instance.CheckStat(currentlevel) == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider i_other)
    {
        if (i_other.gameObject.CompareTag(TagManager.Player))
        {
            InventorySystem.Instance.MarkCollected(currentlevel);
            Destroy(gameObject);
        }
    }
}