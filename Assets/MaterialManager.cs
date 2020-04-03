using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialManager : MonoBehaviour
{
    public enum material{ mat1, mat2};

    public List<Material> materialList;

    public Material GetMaterial(int i_index)
    {
        if (i_index < materialList.Count)
        {
            return materialList[i_index];
        }
        else
            throw new Exception("Material index out of range");
    }

}
