using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubieObject : MonoBehaviour
    {
        public CubeLayerMask Down = CubeLayerMask.down;
        public LayerMask PlaneLayerMask;
        public TileObject VolumetricTile;

        public bool TryEnterCubie(CubeLayerMask i_direction)
        {
            TileObject tile = GetPlanimetricTile(-i_direction);
            return tile.TryEnterTile(i_direction) && VolumetricTile.TryEnterTile(i_direction);
        }

        public bool TryExitCubie(CubeLayerMask i_direction)
        {
            TileObject tile = GetPlanimetricTile(i_direction);
            return tile.TryExitTile(i_direction) && VolumetricTile.TryExitTile(i_direction);
        }

        public TileObject GetPlanimetricTile(CubeLayerMask i_direction)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, i_direction.ToVector3(), out hit, Mathf.Infinity, PlaneLayerMask))
                Debug.DrawRay(transform.position, i_direction.ToVector3() * hit.distance, Color.blue);
            return hit.transform.GetComponentInChildren<TileObject>();
        }
    }

}