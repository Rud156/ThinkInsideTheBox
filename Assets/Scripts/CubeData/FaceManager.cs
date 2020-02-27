using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

[ExecuteAlways]
public class FaceManager : MonoBehaviour
{
    public List<TileFunction> faceFunctionList;
    public List<ReachEvent> reachEventList;
    

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
        
    }
}
