using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestCubeRotator : MonoBehaviour
    {
        private const string CenterBlock = "CenterBlock";
        private const string ParentTag = "SideCubeParent";

        [System.Serializable]
        public struct CubeSide
        {
            [Header("Center")] public Transform centerCube;
            public Transform centerRayCastPosition;

            [Header("Direction and Distance")] public float raycastDistance;
            public Vector3 rayCastDirection;
        }

        public Transform cubeParent;
        public int sideDifference;
        public float rotationIncrementAngle = 90;
        public float lerpSpeed;
        public List<CubeSide> cubeSides;

        private Vector3 _startRotation;
        private Vector3 _targetRotation;
        private float _lerpAmount;

        private List<Transform> _targetCubes;
        private GameObject _fakeParent;

        private bool _lerpRunning;

        #region Unity Functions

        private void Start()
        {
            _targetCubes = new List<Transform>();
            _lerpRunning = false;
        }

        private void Update()
        {
            if (_lerpRunning)
            {
                LerpFakeCubeSide();
                return;
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandleSideRotation(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                HandleSideRotation(0, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HandleSideRotation(1, -1);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                HandleSideRotation(1, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                HandleSideRotation(2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                HandleSideRotation(2, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                HandleSideRotation(3, -1);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                HandleSideRotation(3, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                HandleSideRotation(4, -1);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                HandleSideRotation(4, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                HandleSideRotation(5, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                HandleSideRotation(5, 1);
            }
        }

        #endregion

        #region Utility Functions

        private void LerpFakeCubeSide()
        {
            _fakeParent.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(_startRotation),
                Quaternion.Euler(_targetRotation),
                _lerpAmount
            );

            _lerpAmount += lerpSpeed * Time.deltaTime;
            if (_lerpAmount >= 0.97f)
            {
                _lerpRunning = false;
                _fakeParent.transform.rotation = Quaternion.Euler(_targetRotation);

                foreach (Transform targetCube in _targetCubes)
                {
                    targetCube.SetParent(cubeParent);
                }

                Destroy(_fakeParent);
            }
        }

        private void HandleSideRotation(int index, int rotaionDirection)
        {
            CubeSide cubeSize = cubeSides[index];
            Vector3 centerPosition = cubeSize.centerRayCastPosition.position;
            Vector3 direction = cubeSize.rayCastDirection;

            bool xIsNotZero = direction.x != 0;
            bool yIsNotZero = direction.y != 0;
            bool zIsNotZero = direction.z != 0;

            List<Vector3> positions = new List<Vector3>();

            // Used to find all the raycast positions
            for (int i = -sideDifference; i <= sideDifference; i++)
            {
                for (int j = -sideDifference; j <= sideDifference; j++)
                {
                    float xValue = 0;
                    float yValue = 0;
                    float zValue = 0;
                    if (xIsNotZero)
                    {
                        xValue = 0;
                        yValue = i;
                        zValue = j;
                    }
                    else if (yIsNotZero)
                    {
                        xValue = i;
                        yValue = 0;
                        zValue = j;
                    }
                    else if (zIsNotZero)
                    {
                        xValue = i;
                        yValue = j;
                        zValue = 0;
                    }

                    Vector3 finalPosition = centerPosition + new Vector3(xValue, yValue, zValue);
                    positions.Add(finalPosition);
                }
            }


            _targetCubes.Clear();
            _fakeParent = new GameObject();

            // Find all cubes which are hit by the raycast
            foreach (Vector3 position in positions)
            {
                bool raycastSuccess = Physics.Raycast(
                    position, cubeSize.rayCastDirection,
                    out RaycastHit hit, cubeSize.raycastDistance
                );
                if (raycastSuccess)
                {
                    _targetCubes.Add(hit.collider.transform.parent);
                    if (hit.collider.CompareTag(CenterBlock))
                    {
                        _startRotation = hit.collider.transform.parent.eulerAngles;
                        _fakeParent.transform.position = hit.collider.transform.position;
                        _fakeParent.transform.rotation = Quaternion.Euler(_startRotation);
                    }
                }
            }

            // Set the fake parent for rotation
            foreach (Transform targetCube in _targetCubes)
            {
                targetCube.SetParent(_fakeParent.transform);
            }

            // Set the target rotation
            if (xIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.x += rotationIncrementAngle * rotaionDirection;
            }
            else if (yIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.y += rotationIncrementAngle * rotaionDirection;
            }
            else if (zIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.z += rotationIncrementAngle * rotaionDirection;
            }

            _lerpRunning = true;
            _lerpAmount = 0;
        }

        #endregion
    }
}
