using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CubeData
{
    public class CubieObject : MonoBehaviour
    {
        public static readonly Color[] CubeColors = { Color.cyan, Color.red, Color.blue, Color.green, Color.yellow, Color.black, Color.white };

        public const float LENGTH = 1;

        public Renderer PlaneU;
        public Renderer PlaneD;
        public Renderer PlaneL;
        public Renderer PlaneR;
        public Renderer PlaneF;
        public Renderer PlaneB;

        public Cubie CubieData;

        public void DrawCubie()
        {
            Tint(CubieData);
            SetText(CubieData);
            Hide(CubieData);
        }

        public void Tint(Cubie i_cubie)
        {
            PlaneR.material.color = CubeColors[Mathf.Abs(i_cubie.x) / 10];
            PlaneL.material.color = CubeColors[Mathf.Abs(i_cubie.x) / 10];
            PlaneU.material.color = CubeColors[Mathf.Abs(i_cubie.y) / 10];
            PlaneD.material.color = CubeColors[Mathf.Abs(i_cubie.y) / 10];
            PlaneF.material.color = CubeColors[Mathf.Abs(i_cubie.z) / 10];
            PlaneB.material.color = CubeColors[Mathf.Abs(i_cubie.z) / 10];
        }

        public void Hide(Cubie i_cubie)
        {
            PlaneR.gameObject.SetActive(i_cubie.x > 0);
            PlaneL.gameObject.SetActive(i_cubie.x < 0);
            PlaneU.gameObject.SetActive(i_cubie.y > 0);
            PlaneD.gameObject.SetActive(i_cubie.y < 0);
            PlaneF.gameObject.SetActive(i_cubie.z > 0);
            PlaneB.gameObject.SetActive(i_cubie.z < 0);
        }

        public void Place(Cubie i_cubie)
        {
            Vector3 pos = new Vector3(
                i_cubie.x > 0 ? LENGTH : -LENGTH,
                i_cubie.y > 0 ? LENGTH : -LENGTH,
                i_cubie.z > 0 ? LENGTH : -LENGTH);
            pos.x = i_cubie.x == 0 ? 0 : pos.x;
            pos.y = i_cubie.y == 0 ? 0 : pos.y;
            pos.z = i_cubie.z == 0 ? 0 : pos.z;
            transform.localPosition = pos;
        }

        public void SetText(Cubie i_cubie)
        {
            PlaneU.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.y) % 10).ToString();
            PlaneD.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.y) % 10).ToString();
            PlaneF.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.z) % 10).ToString();
            PlaneB.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.z) % 10).ToString();
            PlaneL.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.x) % 10).ToString();
            PlaneR.GetComponentInChildren<TMP_Text>().text = (Mathf.Abs(i_cubie.x) % 10).ToString();
        }
    }

}
