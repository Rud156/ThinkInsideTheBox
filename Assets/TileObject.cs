using System;
using System.Collections;
using System.Collections.Generic;
using CubeData;
using UnityEngine;

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
        // Return another CubeLayerMask if the path is accessible but the direction changes.

        Vector3 moveDir = i_direction.ToVector3();

        //if(AccessAvailable(moveDir, false))
        //{
        //    Vector3 playerDir = moveDir * -1;    //mark the direction of the player relative to this tile
        //    if(AccessAvailable(playerDir, true))    //check if player can
        //}
        Vector3 playerDir = moveDir * -1;
        if(AccessAvailable(playerDir, true))
        {

        }
        

        throw new NotImplementedException();
    }

    private bool AccessAvailable(Vector3 i_dir, bool getIn)
    {
        if (i_dir.magnitude > 1)
        {
            Debug.LogError("Invalid direction vector! Returned with false");
            return false;
        }

        int axis = GetDirection(i_dir);

        if(getIn)
        {
            switch (axis) //0 - x; 1 - y; 2 - z
            {
                case 0:
                    if (i_dir.x > 0)
                        return right_in;
                    else
                        return left_in;
                case 1:
                    if (i_dir.y > 0)
                        return up_in;
                    else
                        return down_in;
                case 2:
                    if (i_dir.z > 0)
                        return forward_in;
                    else
                        return back_in;
            }
        }
        else
        {
            switch (axis) //0 - x; 1 - y; 2 - z
            {
                case 0:
                    if (i_dir.x > 0)
                        return right_out;
                    else
                        return left_out;
                case 1:
                    if (i_dir.y > 0)
                        return up_out;
                    else
                        return down_out;
                case 2:
                    if (i_dir.z > 0)
                        return forward_out;
                    else
                        return back_out;
            }
        }
        
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
