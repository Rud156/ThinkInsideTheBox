using System;
using System.Collections;
using System.Collections.Generic;
using CubeData;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CubeLayerMask GetMoveDirection(CubeLayerMask i_direction)
    {
        // Return CubeLayerMask.Zero if the path is blocked
        // Return i_direction if the path is accessible and the direction keeps
        // Return another CubeLayerMask if the path is accessible but the direction changes
        throw new NotImplementedException();
    }
}
