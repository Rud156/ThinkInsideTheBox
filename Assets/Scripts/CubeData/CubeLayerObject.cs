using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubeLayerObject : MonoBehaviour
    {
        private List<CubieObject> m_cubieObjects = new List<CubieObject>();
        private CubeObject m_cubeObject;
        private CubeLayerMask m_layerMask;

        public void SetupLayer(CubeObject i_cubeObject, CubeLayerMask i_layerMask)
        {
            m_cubeObject = i_cubeObject;
            m_layerMask = i_layerMask;
            foreach (var cubie in m_cubeObject.CubeData.Cubies)
            {
                CreateCubie(cubie);
            }
            HideCubeLayer(i_layerMask);
            PlaceCube();
            DrawCube();
        }

        public void UpdateLayer(CubeObject i_cubeObject)
        {
            m_cubeObject = i_cubeObject;
            InjectData(i_cubeObject.CubeData);
            DrawCube();
        }

        private void PlaceCube()
        {
            int n = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (m_cubieObjects[n].isActiveAndEnabled) {
                            m_cubieObjects[n].transform.localPosition = new Vector3(
                            (k - 1) * CubieObject.LENGTH,
                            (i - 1) * CubieObject.LENGTH,
                            (j - 1) * CubieObject.LENGTH);
                        }
                        n++;
                    }
                }
            }
        }

        private void HideCubeLayer(CubeLayerMask i_layerMask)
        {
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        bool result = false;
                        if (i_layerMask.x != 0 && k == i_layerMask.x + 1 ||
                            i_layerMask.y != 0 && i == i_layerMask.y + 1 ||
                            i_layerMask.z != 0 && j == i_layerMask.z + 1)
                            result = true;
                        m_cubieObjects[index].gameObject.SetActive(result);
                        index++;
                    }
                }
            }
        }

        private void DrawCube()
        {
            foreach (var cubieObject in m_cubieObjects)
            {
                if (cubieObject.isActiveAndEnabled)
                {
                    cubieObject.DrawCubie();
                }
            }
        }

        private void InjectData(Cube i_cube)
        {
            int i = 0;
            foreach (var cubie in i_cube.Cubies)
            {
                m_cubieObjects[i].CubieData = cubie;
                i++;
            }
        }

        private void CreateCubie(Cubie i_cubie)
        {
            var obj = Instantiate(m_cubeObject.CubieObject, transform);
            obj.CubieData = i_cubie;
            m_cubieObjects.Add(obj);
        }
    }

}
