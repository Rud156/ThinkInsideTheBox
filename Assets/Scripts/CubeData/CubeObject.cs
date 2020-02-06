using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    public class CubeObject : MonoBehaviour
    {
        private Cube m_cube = new Cube();
        public CubieObject CubieObject;
        public Cube CubeData { get { return m_cube; } }
        public List<CubeLayerMask> LayerMasks = new List<CubeLayerMask>();
        private List<CubeLayerObject> m_layerObjects = new List<CubeLayerObject>();

        private void Awake()
        {
            foreach (var layer in LayerMasks)
            {
                var obj = new GameObject("Layer").AddComponent<CubeLayerObject>();
                obj.transform.parent = transform;
                obj.SetupLayer(this, layer);
                m_layerObjects.Add(obj);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_cube.RotYn90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                m_cube.RotYp90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_cube.RotXp90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_cube.RotXn90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                m_cube.RotZp90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                m_cube.RotZn90d(2);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer(this);
                }
                Debug.Log(m_cube.HasFinished());
            }
        }
    }

}
