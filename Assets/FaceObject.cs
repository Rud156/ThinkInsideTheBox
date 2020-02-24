﻿using System;
using System.Collections;
using System.Collections.Generic;
using CubeData;
using UnityEngine;
public enum TileFunction
{
    Water, Turn, Ramp, Exit, Wall, Default, Custom
}

public enum TurnDirection
{
    Left, Right, Forward, Back
}

public class FaceObject : MonoBehaviour
{
    //  Mark the accessable directions starting from this tile object.
    [Header("Access")]
    public bool forward;
    public bool back;
    public bool right;
    public bool left;
    public bool up;
    public bool down;

    [Header("Facet Function")]
    public TileFunction tileType = TileFunction.Default;

    public TurnDirection turnTo = TurnDirection.Left;   //default turn to left if this is a turn-facet
    // Start is called before the first frame update
    private void Awake()
    {
        if (tileType == TileFunction.Ramp)  //ramp has to be placed with forward pointing downwards
        {
            forward = true; //forward is the ramping up side
            back = true;
            right = true;
            left = true;
            up = false;  
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
        else if (tileType == TileFunction.Turn)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;
        }
        else if (tileType==TileFunction.Wall)
        {
            forward = false;
            back = false;
            right = false;
            left = false;
            up = false;
            down = false;
        }
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
                if (playerDir == CubeLayerMask.forward.ToVector3())
                    return CubeLayerMask.up;
                else
                    return i_direction;
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

    public (CubeLayerMask, bool) TryChangeDirection(CubeLayerMask i_direction)
    {
        Vector3 moveDir = i_direction.ToVector3();
        Vector3 playerDir = GetPlayerRelativeDir(moveDir); //mark the direction of the player relative to this tile
        if (AccessAvailable(playerDir))
        {
            if (tileType == TileFunction.Ramp)
            {
                //The condition where the player climbs up a ramp
                //Debug.Log("Go up ramp - change direction Up");
                if (i_direction.ToVector3() == this.transform.forward)   //see if the player is climbing the ramp
                    return (CubeLayerMask.up, false);
                else
                    return (i_direction, false);
            }
            else if (tileType == TileFunction.Turn)
            {
                //The condition where the player meets a turning face
                if (turnTo == TurnDirection.Forward)
                {
                    return (new CubeLayerMask(this.transform.forward), true);
                }
                else if (turnTo == TurnDirection.Back)
                {
                    return (new CubeLayerMask(-this.transform.forward), true);
                }
                else if(turnTo == TurnDirection.Left)
                {
                    return (new CubeLayerMask(-this.transform.right), true);
                }
                else
                {
                    return (new CubeLayerMask(this.transform.right), true);
                }
            }
            else
            {
                //The condition where the player can get through the face
                //Debug.Log("Don't change dir");
                return (i_direction, false);
            }

        }
        else
        {
            //The condition where the player gets blocked
            //Debug.Log(this.transform.name);
            //Debug.Log(gameObject.transform.parent.name + "Blocked");
            return (CubeLayerMask.Zero, true);
        }

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
