using UnityEngine;

[CreateAssetMenu(fileName = "FaceData", menuName = "ScriptableObjects/FaceDataScriptableObject", order = 1)]
public class FaceDataScriptableObject : ScriptableObject
{
    public string prefabName;
    public TileFunction faceType;

    [Header("Access")]
    public bool forward;
    public bool back;
    public bool right;
    public bool left;
    public bool up;
    public bool down;
    
    [Header("Face-specific")]
    public TurnDirection turnTo = TurnDirection.Left;   //default turn to left if this is a turn-facet
}