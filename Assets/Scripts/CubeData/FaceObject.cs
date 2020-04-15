using CubeData;
using WorldCube;
using System.Collections;
using Player;
using Scenes.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum TileFunction
{
    Turn,
    Ramp,
    Wall,
    Custom,
    None
}

public enum TurnDirection
{
    Forward,
    Right,
    Back,
    Left
}

public enum ReachEvent
{
    None,
    Window,
    Exit
}

public enum MaterialType
{
    Dirt,
    Stone,
    Wall_Bottom,
    Wall_Mid,
    Wall_Top,
    Wood,
    Grass
}

[ExecuteAlways]
public class FaceObject : MonoBehaviour
{
    [Header("Facet Function")] public TileFunction faceType = TileFunction.None;
    //public bool applyChange = false;

    [Header("Face-specific")] public TurnDirection turnTo = TurnDirection.Forward; //default turn to left if this is a turn-facet
    public bool showWallFace = true;
    public MaterialType materialType;
    public GameObject turnArrow;
    public GameObject water;
    private GameObject arrow_instantiated;
    private GameObject water_instantiated;
    private GameObject exit_instantiated;

    //  Mark the accessable directions starting from this tile object.
    [Header("Custom Access")] public bool forward;
    public bool back;
    public bool right;
    public bool left;
    public bool up;
    public bool down;

    [Header("EventAfterReaching")] public ReachEvent faceEvent;
    public int faceExitWorldIndex;
    public float sceneReloadDelay = 1.2f;

    public delegate void LoadLevel();
    public static event LoadLevel OnLoaded;

    private GameObject instantiated_arrow;
    private MaterialManager matManager;

    private void Awake()
    {
        if (this.isActiveAndEnabled)
            StartCoroutine(ClearAddOns());
    }

    private void Start()
    {
        matManager = GameObject.FindGameObjectWithTag("MaterialManager").GetComponent<MaterialManager>();
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
        string cubie_name = transform.parent.name;
        if (faceEvent == ReachEvent.Window)
        {
            Debug.Log("Player Died. Reloading the scene");
            StartCoroutine(SwitchLevel(SceneManager.GetActiveScene().buildIndex, true));
        }
        else if (faceEvent == ReachEvent.Exit)
        {
            Debug.Log("Player Won");
            StartCoroutine(SwitchLevel(faceExitWorldIndex, false));
        }
    }

    IEnumerator SwitchLevel(int i_index, bool i_isReload)
    {
        if (i_isReload)
        {
            yield return new WaitForSeconds(sceneReloadDelay);
        }

        OnLoaded?.Invoke();
        MainSceneController.Instance.CheckAndDisconnectSocket();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(i_index);
    }

    public CubeLayerMask TryChangeDirection(CubeLayerMask i_direction)
    {
        Vector3 moveDir = i_direction.ToVector3();
        Vector3 playerDir = GetPlayerRelativeDir(new CubeLayerMask(moveDir * -1)); //mark the direction of the player relative to this tile

        if (AccessAvailable(playerDir))
        {
            if (faceType == TileFunction.Ramp)
            {
                //The condition where the player climbs up a ramp
                //Debug.Log("Go up ramp - change direction Up");
                if (i_direction.ToVector3() == this.transform.forward) //see if the player is climbing the ramp
                    return (new CubeLayerMask(-this.transform.up));
                else if (i_direction.ToVector3() == -this.transform.up)
                {
                    return (new CubeLayerMask(-this.transform.forward));
                }
                else
                    return (i_direction);
            }
            else if (faceType == TileFunction.Turn)
            {
                //The condition where the player meets a turning face
                if (turnTo == TurnDirection.Forward)
                {
                    return (new CubeLayerMask(this.transform.forward));
                }
                else if (turnTo == TurnDirection.Back)
                {
                    return (new CubeLayerMask(-this.transform.forward));
                }
                else if (turnTo == TurnDirection.Left)
                {
                    return (new CubeLayerMask(-this.transform.right));
                }
                else
                {
                    return (new CubeLayerMask(this.transform.right));
                }
            }
            else if ((faceType == TileFunction.None || faceType == TileFunction.Custom)
                     && i_direction.ToVector3() != Vector3.up)
            {
                return (i_direction);
            }
            else
            {
                //The condition where the player can get through the face
                //Debug.Log("Don't change dir");
                return (i_direction);
            }
        }
        else
        {
            //The condition where the player gets blocked
            //Debug.Log(this.transform.name);
            //Debug.Log(gameObject.transform.parent.name + "Blocked");
            return (CubeLayerMask.Zero);
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
        Debug.Log(i_dir);
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

    private Vector3 GetPlayerRelativeDir(CubeLayerMask i_direction)
    {
        if (i_direction == new CubeLayerMask(this.transform.forward))
            return new Vector3(0, 0, 1);
        else if (i_direction == new CubeLayerMask(-this.transform.forward))
            return new Vector3(0, 0, -1);
        else if (i_direction == new CubeLayerMask(this.transform.right))
            return new Vector3(1, 0, 0);
        else if (i_direction == new CubeLayerMask(-this.transform.right))
            return new Vector3(-1, 0, 0);
        else if (i_direction == new CubeLayerMask(this.transform.up))
            return new Vector3(0, 1, 0);
        else if (i_direction == new CubeLayerMask(-this.transform.up))
            return new Vector3(0, -1, 0);

        Debug.LogError("Not valid direction input");
        return Vector3.zero;
    }

    public void SetFaceFunction(TileFunction i_function)
    {
        faceType = i_function;
    }

    private void LoadFaceData()
    {
        if (showWallFace)
        {
            SetGroundVisibility(true);
        }
        else
        {
            SetGroundVisibility(false);
        }

        //Load face access related prefabs and data
        if (faceType == TileFunction.Turn)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;
            //Turn the face into the right angle (opposite of the tile/face)
            GameObject arrow_instance = null;
            if (turnArrow)
            {
                //showWallFace = true;
                float rotation_y = 90f * (int) turnTo;
                arrow_instance = Instantiate(turnArrow, this.transform) as GameObject;

                arrow_instance.transform.localEulerAngles = new Vector3(0f, rotation_y, 180f);
                instantiated_arrow = arrow_instance;
                //if (!instantiated_arrow)
                //{
                //    arrow_instance = Instantiate(turnArrow, this.transform) as GameObject;

                //    arrow_instance.transform.localEulerAngles = new Vector3(0f, rotation_y, 180f);
                //    instantiated_arrow = arrow_instance;
                //} 
                //else
                //{
                //    instantiated_arrow.transform.localEulerAngles = new Vector3(0f, rotation_y, 180f);
                //}
            }

            arrow_instantiated = arrow_instance;
            //GetComponent<MeshRenderer>().enabled = false;
            SetGroundVisibility(false);
        }
        else if (faceType == TileFunction.Wall)
        {
            forward = true;
            back = true;
            right = true;
            left = true;
            up = false;
            down = false;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }

            //if (showWallFace)
            //    SetGroundVisibility(true);
            //else
            //    SetGroundVisibility(false);
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
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        //Load event-related prefabs
        if (faceEvent == ReachEvent.Window)
        {
            GameObject water_instance = null;
            if (water)
            {
                water_instance = Instantiate(water, this.transform) as GameObject;
                //SetGroundVisibility(false);
                //float rotation_y = 90f * (int)turnTo;
                water_instance.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
                //Debug.Log(water_instance.tag);
            }

            SetGroundVisibility(false);
            water_instantiated = water_instance;
        }
    }

    private void LoadMaterialInstance()
    {
        if (matManager == null)
        {
            matManager = GameObject.FindGameObjectWithTag("MaterialManager").GetComponent<MaterialManager>();
        }

        if (matManager)
        {
            Material mat = matManager.GetMaterial((int) materialType);
            this.transform.GetChild(0).GetComponentInChildren<Renderer>().material = mat;
        }
    }

    private void OnValidate()
    {
        if (this.isActiveAndEnabled)
            StartCoroutine(ClearAddOns());
    }

    IEnumerator ClearAddOns()
    {
        yield return null; //new WaitForEndOfFrame();

        Transform move_sign = transform.Find("Move_Sign(Clone)");
        
        foreach(Transform child in transform)
        {
            if(child.CompareTag("WaterHole"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        if (move_sign)
        {
            DestroyImmediate(move_sign.gameObject);
            DestroyImmediate(arrow_instantiated);
            arrow_instantiated = null;
        }


        if (showWallFace)
        {
            SetGroundVisibility(true);
            //Debug.Log("Show wall faces");
        }

        LoadFaceData();
        LoadMaterialInstance();
    }

    private void SetGroundVisibility(bool i_visible)
    {
        Transform ground_face = this.transform.Find("Grass Ground");

        if (ground_face)
        {
            MeshRenderer[] renderers = ground_face.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer var in renderers)
            {
                var.enabled = i_visible;
            }

            //ground_face.GetComponent<MeshRenderer>().enabled = i_visible;
            //Debug.Log(ground_face.GetComponentInChildren<MeshRenderer>().name);
        }
    }
}