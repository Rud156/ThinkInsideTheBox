using System;
using System.Collections;
using System.Collections.Generic;
using CubeData;
using UnityEngine;
public enum TileFunction
{
    Turn, Ramp, Wall, Custom, None
}

public enum TurnDirection
{
    Forward, Right, Back, Left
}

public enum ReachEvent
{
    None, Water, Exit
}

public class FaceObject : MonoBehaviour
{
    [Header("Facet Function")]
    public TileFunction faceType = TileFunction.None;

    [Header("Face-specific")]
    public TurnDirection turnTo = TurnDirection.Forward;   //default turn to left if this is a turn-facet
    public GameObject turnArrow;
    public GameObject wallTile;

    //  Mark the accessable directions starting from this tile object.
    [Header("Custom Access")]
    public bool forward;
    public bool back;
    public bool right;
    public bool left;
    public bool up;
    public bool down;

    [Header("EventAfterReaching")]
    public ReachEvent faceEvent;
    // Start is called before the first frame update
    private void Awake()
    {
        #region LoadFaceData
        if (faceType == TileFunction.Turn)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;
            //Turn the face into the right angle (opposite of the tile/face)
            GameObject arrow_instance;
            if (turnArrow)
            {
                arrow_instance = Instantiate(turnArrow, this.transform) as GameObject;
                //arrow_instance.transform.position = this.transform.position;
                //arrow_instance.transform.rotation = this.transform.rotation;

                float rotation_y = 90f * (int)turnTo;
                float rotation_z = 180f;
                arrow_instance.transform.eulerAngles = this.transform.eulerAngles;
                arrow_instance.transform.eulerAngles += new Vector3(0f, 90f, 180f);

                //Vector3 turnArrowAngle = arrow_instance.transform.eulerAngles;
                //arrow_instance.transform.eulerAngles = turnArrowAngle;  // = arrowQuarternion;
                //Quaternion arrowQuarternion = Quaternion.Euler(turnArrowAngle);
                //arrow_instance.transform.eulerAngles = turnArrowAngle;  // = arrowQuarternion;
            }
                
            GetComponent<MeshRenderer>().enabled = false;
            
            
        }
        else if (faceType == TileFunction.Wall)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;
            

            //if (wallTile)
                //Instantiate(wallTile, this.transform.position, this.transform.rotation, this.transform);
        }
        else if (faceType == TileFunction.None)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = true;
            down = true;
            GetComponent<MeshRenderer>().enabled = false;
        }
        #endregion
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
            if (faceType == TileFunction.Ramp)
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

    public void OnPlayerEnter(Dummy dummy)
    {
        //throw new NotImplementedException();
    }

    public (CubeLayerMask, bool) TryChangeDirection(CubeLayerMask i_direction)
    {
        Vector3 moveDir = i_direction.ToVector3();
        Vector3 playerDir = GetPlayerRelativeDir(moveDir); //mark the direction of the player relative to this tile
        if (AccessAvailable(playerDir))
        {
            if (faceType == TileFunction.Ramp)
            {
                //The condition where the player climbs up a ramp
                //Debug.Log("Go up ramp - change direction Up");
                if (i_direction.ToVector3() == this.transform.forward)   //see if the player is climbing the ramp
                    return (new CubeLayerMask(-this.transform.up), false);
                else if (i_direction.ToVector3() == -this.transform.up)
                {
                    return (new CubeLayerMask(-this.transform.forward), false);
                }
                else
                    return (i_direction, false);
            }
            else if (faceType == TileFunction.Turn)
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
            else if ((faceType == TileFunction.None || faceType == TileFunction.Custom) 
                && i_direction.ToVector3() != Vector3.up)
            {
                return (CubeLayerMask.down, false);
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
                    return down;
                }
                else
                {
                    //Debug.Log("Player on Down");
                    return up;
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
