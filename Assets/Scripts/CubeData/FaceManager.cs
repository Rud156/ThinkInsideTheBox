using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public struct FaceInfo
{
    FaceObject faceObject;
    TileFunction faceFunction;
    ReachEvent reachEvent;
}

public enum FaceOrder
{
    back = 0, forward, up, down, right, left
}

public enum CubieType
{
    Corner, Edge, Face, Ramp
}

[ExecuteAlways]
public class FaceManager : MonoBehaviour
{
    public CubieType cubieType;
    public bool applyChange = false;

    public List<GameObject> cubiePrefabs;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if(applyChange)
        {
            Debug.Log("Exe!");
            UpdateCubie();
        }
        
    }

    private void UpdateCubie()
    {
        GameObject updatedObject = (GameObject)PrefabUtility.InstantiatePrefab(cubiePrefabs[(int)cubieType]);
        updatedObject.transform.position = this.transform.position;
        updatedObject.transform.rotation = this.transform.rotation;
        updatedObject.transform.localScale = this.transform.localScale;

        updatedObject.transform.parent = this.transform.parent;
        StartCoroutine(DestroyOldObject());
    }

    IEnumerator DestroyOldObject()
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(this.gameObject);
    }
}
