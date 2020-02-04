using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubeObject : MonoBehaviour
    {
        public CubieObject CubieObject;
        private List<CubieObject> CubieObjects = new List<CubieObject>();
        private Cube cube = new Cube();

        private void Start()
        {
            foreach (var cubie in cube.Cubies)
            {
                CreateCubie(cubie);
            }
            PlaceCube();
            DrawCube(cube);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                cube.RotYn90d(2);
                InjectData(cube);
                DrawCube(cube);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                cube.RotYp90d(2);
                InjectData(cube);
                DrawCube(cube);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                cube.RotXp90d(1);
                InjectData(cube);
                DrawCube(cube);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                cube.RotXn90d(1);
                InjectData(cube);
                DrawCube(cube);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                cube.RotZp90d(0);
                InjectData(cube);
                DrawCube(cube);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                cube.RotZn90d(0);
                InjectData(cube);
                DrawCube(cube);
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
            var obj = Instantiate<CubieObject>(CubieObject, transform);
            obj.CubieData = i_cubie;
            CubieObjects.Add(obj);
        }
    }

}
