using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CubeData
{
    public class CubeObject : MonoBehaviour
    {
        private Cube m_cube = new Cube();
        public CubieObject CubieObject;
        public Cube CubeData { get { return m_cube; } }
        public List<CubeLayerMask> LayerMasks = new List<CubeLayerMask>();
        private List<CubeLayerObject> m_layerObjects = new List<CubeLayerObject>();
        private Vector3 m_lastUp = Vector3.up;
        private CubeLayerMask m_rotatingLayer = CubeLayerMask.Zero;
        private bool m_pauseInput = false;

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

        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(0, 1, 0), true);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(0, 1, 0), false);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(1, 0, 0), false);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(1, 0, 0), true);
            }
            if (Input.GetKeyDown(KeyCode.W)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.W))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(-1, 0, 0), false);
            }
            if (Input.GetKeyDown(KeyCode.S)) m_pauseInput = false;
            if (Input.GetKey(KeyCode.S))
            {
                if (m_pauseInput) return;
                RotateCube(new CubeLayerMask(-1, 0, 0), true);
            }
        }

        private void RotateCube(CubeLayerMask i_cubeLayerMask, bool i_isClockwise)
        {
            bool isRotating = false;
            m_layerObjects.ForEach(n => isRotating |= n.IsRotating);
            if (isRotating && !(m_rotatingLayer ^ i_cubeLayerMask))
            {
                return;
            }
            m_rotatingLayer = i_cubeLayerMask;
            bool dirty = false;
            foreach (var layerObject in m_layerObjects)
            {
                dirty |= layerObject.RotateLayer(i_cubeLayerMask, i_isClockwise);
                isRotating |= layerObject.IsRotating;
            }
            if (dirty)
            {
                m_pauseInput = true;
                RotateCubeData(i_cubeLayerMask, i_isClockwise);
                foreach (var layerObject in m_layerObjects)
                {
                    layerObject.UpdateLayer();
                }
            }
        }

        private void RotateCubeData(CubeLayerMask i_cubeLayerMask, bool i_isClockwise)
        {
            Assert.IsTrue(i_cubeLayerMask.IsValid());
            if (i_cubeLayerMask == CubeLayerMask.Zero)
                return;
            if (i_cubeLayerMask.x != 0)
                if (i_isClockwise)
                    m_cube.RotXp90d(i_cubeLayerMask.x + 1);
                else
                    m_cube.RotXn90d(i_cubeLayerMask.x + 1);
            else if (i_cubeLayerMask.y != 0)
                if (i_isClockwise)
                    m_cube.RotYp90d(i_cubeLayerMask.y + 1);
                else
                    m_cube.RotYn90d(i_cubeLayerMask.y + 1);
            else if (i_cubeLayerMask.z != 0)
                if (i_isClockwise)
                    m_cube.RotZp90d(i_cubeLayerMask.z + 1);
                else
                    m_cube.RotZn90d(i_cubeLayerMask.z + 1);
        }
    }

}
