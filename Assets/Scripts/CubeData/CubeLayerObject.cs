using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubeLayerObject : MonoBehaviour
    {
        public CubieObject CubieObject;
        private List<CubieObject> CubieObjects = new List<CubieObject>();
        private Cube cube = new Cube();

        private void CreateCubie(Cubie i_cubie)
        {
            var obj = Instantiate<CubieObject>(CubieObject, transform);
            obj.CubieData = i_cubie;
            CubieObjects.Add(obj);
        }
    }
}

