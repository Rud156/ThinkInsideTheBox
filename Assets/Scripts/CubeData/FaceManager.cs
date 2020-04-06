using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

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
    Corner, Edge, Face, Ramp, Exit
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
        
        if (applyChange)
        {
            //Debug.Log("Exe!");
            UpdateCubie();
        }

    }

    private void UpdateCubie()
    {
#if UNITY_EDITOR
        GameObject updatedObject = (GameObject)PrefabUtility.InstantiatePrefab(cubiePrefabs[(int)cubieType]);
        if(updatedObject)
        {
            updatedObject.transform.position = this.transform.position;
            updatedObject.transform.rotation = this.transform.rotation;
            updatedObject.transform.localScale = this.transform.localScale;
            updatedObject.transform.parent = this.transform.parent;
            StartCoroutine(DestroyOldObject());
        }
        //else
        //{
        //    throw new Exception("Updating cubie failed");
        //}

#endif
    }

    IEnumerator DestroyOldObject()
    {
        yield return null;//new WaitForEndOfFrame();
        DestroyImmediate(this.gameObject);
    }
}
