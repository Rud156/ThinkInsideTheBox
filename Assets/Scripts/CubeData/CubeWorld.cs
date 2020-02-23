using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CubeData
{
    public static class CubeWorld
    {
        public const float CUBIE_LENGTH = 1;
        public const int CUBIE_LAYER_MASK = 1 << 15;

        public static bool TryGetNextCubie(Vector3 i_origin, CubeLayerMask i_direction, out CubieObject o_cubie)
        {
            RaycastHit hit;
            o_cubie = null;
            if (Physics.Raycast(i_origin, i_direction.ToVector3(), out hit, Mathf.Infinity, CubeWorld.CUBIE_LAYER_MASK))
            {
                Debug.DrawRay(i_origin, i_direction.ToVector3() * CUBIE_LENGTH, Color.red, 3f, false);
                o_cubie = hit.transform.GetComponent<CubieObject>();
                return true;
            }
            return false;
        }
    }

}
