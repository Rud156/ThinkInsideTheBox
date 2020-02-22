using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TileData
{
    public TileFunction type;
    public AccessDir access_In;
    public AccessDir access_Out;
}

public struct AccessDir
{
    bool forward;
    bool back;
    bool right;
    bool left;
    bool up;
    bool down;
}
