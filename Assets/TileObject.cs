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
    //  Mark the accessable directions starting from this tile object.
    [Header("Access")]
    public bool forward;
    public bool back;
    public bool right;
    public bool left;
    public bool up;
    public bool down;

    [Header("Tile Function")]
    public TileFunction tileType;
    // Start is called before the first frame update
    private void Awake()
    {
        if (tileType == TileFunction.Ramp)
        {
            forward = false;
            back = true;
            right = false;
            left = false;
            up = true;
            down = false;
        }
        else if (tileType == TileFunction.Default)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;
        }
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
        //Debug.Log("i_direction: " + i_direction.ToVector3());

        Vector3 moveDir = i_direction.ToVector3();
        Vector3 playerDir = GetPlayerRelativeDir(moveDir); //mark the direction of the player relative to this tile

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
            //Debug.Log(this.transform.name);
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
                    return right;
                }
                else
                {
                    //Debug.Log("Player on Left");
                    return left;
                }
                    
            case 1:
                if (i_dir.y > 0)
                {
                    //Debug.Log("Player on Up");
                    return up;
                }
                else
                {
                    //Debug.Log("Player on Down");
                    return down;
                }
                    
            case 2:
                if (i_dir.z > 0)
                {
                    //Debug.Log("Player on Forward");
                    return forward;
                }
                else
                {
                    //Debug.Log("Player on Back");
                    return back;
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

    private Vector3 GetPlayerRelativeDir(Vector3 i_dir)
    {
        if (-i_dir == this.transform.forward)
            return new Vector3(0, 0, 1);
        else if (-i_dir == -this.transform.forward)
            return new Vector3(0, 0, -1);
        else if (-i_dir == this.transform.right)
            return new Vector3(1, 0, 0);
        else if (-i_dir == -this.transform.right)
            return new Vector3(-1, 0, 0);
        else if (-i_dir == this.transform.up)
            return new Vector3(0, 1, 0);
        else if (-i_dir == -this.transform.up)
            return new Vector3(0, -1, 0);

        Debug.LogError("Not valid direction input");
        return Vector3.zero;
    }
}
