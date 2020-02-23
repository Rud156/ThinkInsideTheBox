using System;
using System.Collections;
using System.Collections.Generic;
using CubeData;
using UnityEngine;
public enum TileFunction
{
    Water, Turn, Ramp, Exit, Default
}

public class TileObject : MonoBehaviour
{
    [Header("Access In")]
    public bool forward_in;
    public bool back_in;
    public bool right_in;
    public bool left_in;
    public bool up_in;
    public bool down_in;

    [Header("Access Out")]
    public bool forward_out;
    public bool back_out;
    public bool right_out;
    public bool left_out;
    public bool up_out;
    public bool down_out;

    [Header("Tile Function")]
    public TileFunction tileType;
    // Start is called before the first frame update
    void Start()
    {
        //if (tileType == TileFunction.Ramp)
        //{
        //    forward_in = false;
        //    back_in = true;
        //    right_in = false;
        //    left_in = false;
        //    up_in = true;
        //    down_in = false;
        //}
        //else if(tileType == TileFunction.Default)
        //{
        //    forward_in = true;
        //    back_in = true;
        //    right_in = true;
        //    left_in = true;
        //    up_in = false;
        //    down_in = false;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CubeLayerMask GetMoveDirection(CubeLayerMask i_direction)
    {
        //Debug.Log(this.transform.GetInstanceID());
        // Return CubeLayerMask.Zero if the path is blocked
        // Return i_direction if the path is accessible and the direction keeps
        // Return another CubeLayerMask if the path is accessible but the direction changes.
        Debug.Log("i_direction: " + i_direction.ToVector3());

        Vector3 moveDir = i_direction.ToVector3();
        Vector3 playerDir = moveDir * -1; //mark the direction of the player relative to this tile

        if (AccessAvailable(playerDir))
        {
            if (tileType == TileFunction.Ramp)
            {
                //Debug.Log("Go up ramp - change direction Up");
                return CubeLayerMask.up;
            }
            else
            {
                //Debug.Log("Don't change dir");
                return i_direction;
            }
                
        }
        else
        {
            Debug.Log(this.transform.name);
            //Debug.Log(gameObject.transform.parent.name + "Blocked");
            return CubeLayerMask.Zero;
        }
            

        //throw new NotImplementedException();
    }

    private bool AccessAvailable(Vector3 i_dir)
    {
        if (i_dir.magnitude > 1)
        {
            Debug.LogError("Invalid direction vector! Returned with false");
            return false;
        }

        int axis = GetDirection(i_dir);

        switch (axis) //0 - x; 1 - y; 2 - z
        {
            case 0:
                if (i_dir.x > 0)
                {
                    //Debug.Log("Player on Right");
                    return right_in;
                }
                else
                {
                    //Debug.Log("Player on Left");
                    return left_in;
                }
                    
            case 1:
                if (i_dir.y > 0)
                {
                    //Debug.Log("Player on Up");
                    return up_in;
                }
                else
                {
                    //Debug.Log("Player on Down");
                    return down_in;
                }
                    
            case 2:
                if (i_dir.z > 0)
                {
                    //Debug.Log("Player on Forward");
                    return forward_in;
                }
                else
                {
                    //Debug.Log("Player on Back");
                    return back_in;
                }
                    
        }

        Debug.LogError("Out of conditions error!");
        return false;
    }

    private int GetDirection(Vector3 i_dir)
    {
        if (i_dir.x != 0)
            return 0;
        else if (i_dir.y != 0)
            return 1;
        else if (i_dir.z != 0)
            return 2;

        Debug.LogError("Condition broken");
        return -1;
    }
}
