using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubeObject : MonoBehaviour
    {
        public CubieObject CubieObject;
        private List<CubieObject> CubieObjects = new List<CubieObject>();
        private Cube m_cube = new Cube();
        private static readonly CubeLayerMask m_layerMask = new CubeLayerMask(1, 0, 0);

        private void Start()
        {
            foreach (var cubie in m_cube.Cubies)
            {
                CreateCubie(cubie);
            }
            PlaceCube();
            DrawCube(m_cube);
            HideCubeLayer(m_layerMask);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_cube.RotYn90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                m_cube.RotYp90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_cube.RotXp90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_cube.RotXn90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                m_cube.RotZp90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                m_cube.RotZn90d(2);
                InjectData(m_cube);
                DrawCube(m_cube);
                Debug.Log(m_cube.HasFinished());
            }
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
                        CubieObjects[n].transform.localPosition = new Vector3(
                            (k - 1) * CubieObject.LENGTH,
                            (i - 1) * CubieObject.LENGTH,
                            (j - 1) * CubieObject.LENGTH);
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
                        CubieObjects[index].gameObject.SetActive(result);
                        index++;
                    }
                }
            }
        }

        private void DrawCube(Cube i_cube)
        {
            int i = 0;
            foreach (var cubie in i_cube.Cubies)
            {
                CubieObjects[i].DrawCubie();
                i++;
            }
        }

        private void InjectData(Cube i_cube)
        {
            int i = 0;
            foreach (var cubie in i_cube.Cubies)
            {
                CubieObjects[i].CubieData = cubie;
                i++;
            }
        }

        private void CreateCubie(Cubie i_cubie)
        {
            var obj = Instantiate(CubieObject, transform);
            obj.CubieData = i_cubie;
            CubieObjects.Add(obj);
        }
    }

}
