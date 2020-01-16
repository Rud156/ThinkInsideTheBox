using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace WorldCube
{
    public class CubeController : MonoBehaviour
    {
        private const string CenterBlock = "CenterBlock";
        private const string ParentTag = "SideCubeParent";

        [Header("Cube Data")] public Transform cubeParent;
        public int sideDifference = 1;
        public float rotationIncrementAngle = 90;
        public float lerpSpeed = 1;
        public List<CubeSide> cubeSides;

        [Header("Serial Port Info")] public int serialReadTimeout = 7;
        public float rotationMultiplier = 9;

        private List<Transform> _targetCubes;
        private GameObject _fakeParent;

        private SerialPort _stream;
        private string _lastInputType;
        private List<Vector3> _cubeTargetRotation;
        private List<float> _cubeSideTargetRotation;
        private List<float> _cubeSideCurrentRotation;
        private List<GameObject> _fakeParents;

        #region Unity Functions

        private void Start()
        {
            _targetCubes = new List<Transform>();
            _fakeParents = new List<GameObject>();
            _cubeSideTargetRotation = new List<float>();
            _cubeSideCurrentRotation = new List<float>();
            _cubeTargetRotation = new List<Vector3>();

            _stream = new SerialPort("COM4", 9600);
            _stream.ReadTimeout = serialReadTimeout;
            if (!_stream.IsOpen)
            {
                Debug.Log("Stream Is Closed. Opening");
                _stream.Open();
            }

            for (int i = 0; i < cubeSides.Count; i++)
            {
                _cubeSideCurrentRotation.Add(0);
                _cubeSideTargetRotation.Add(0);
            }
        }

        private void Update()
        {
            ReadFromArduino();
        }

        #endregion

        #region Utility Functions

        #region Arduino

        private void ReadFromArduino()
        {
            string input = _stream.ReadLine();
            switch (input)
            {
                case "A":
                case "B":
                    _lastInputType = input;
                    break;

                default:
                    HandleSecondaryInput(bool.Parse(input));
                    break;
            }
        }

        private void HandleSecondaryInput(bool isPositive)
        {
            if (isPositive)
            {
                switch (_lastInputType)
                {
                    case "A":
                        _cubeSideTargetRotation[0] += rotationMultiplier;
                        break;

                    case "B":
                        _cubeSideTargetRotation[1] += rotationMultiplier;
                        break;
                }
            }
            else
            {
                switch (_lastInputType)
                {
                    case "A":
                        _cubeSideTargetRotation[0] -= rotationMultiplier;
                        break;

                    case "B":
                        _cubeSideTargetRotation[1] -= rotationMultiplier;
                        break;
                }
            }
        }

        private void CheckAndSetParent(int index, bool isPositive)
        {
            bool rotationIsValid = true;
            foreach (int adjacentSide in cubeSides[index].adjacentSides)
            {
                int rotation = (int) _cubeSideTargetRotation[adjacentSide];
                if (rotation % 90 != 0)
                {
                    rotationIsValid = false;
                    break;
                }
            }

            if (rotationIsValid)
            {
                if (isPositive)
                {
                    _cubeSideTargetRotation[index] += rotationMultiplier;
                }
                else
                {
                    _cubeSideTargetRotation[index] -= rotationMultiplier;
                }
            }

            int sideRotation = (int) _cubeSideCurrentRotation[index];
            int targetSideRotation = (int) _cubeSideTargetRotation[index];
            if (sideRotation % 90 == 0 && targetSideRotation % 90 != 0)
            {
                ParentCubeSides(index);
            }
            else if (sideRotation % 90 == 0 && targetSideRotation % 90 == 0)
            {
                UnParentCubeSides(index);
            }
        }

        private void ParentCubeSides(int index)
        {
            CubeSide cubeSize = cubeSides[index];
            Vector3 centerPosition = cubeSize.centerRayCastPosition.position;
            Vector3 direction = cubeSize.rayCastDirection;

            bool xIsNotZero = direction.x != 0;
            bool yIsNotZero = direction.y != 0;
            bool zIsNotZero = direction.z != 0;

            List<Vector3> positions = new List<Vector3>();
            Vector3 _startRotation = Vector3.zero;
            Vector3 _targetRotation = Vector3.zero;

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
            GameObject fakeParent = new GameObject();
            fakeParent.tag = ParentTag;

            // Find all cubes which are hit by the raycast
            foreach (Vector3 position in positions)
            {
                bool raycastSuccess = Physics.Raycast(
                    position, cubeSize.rayCastDirection,
                    out RaycastHit hit, cubeSize.raycastDistance
                );
                if (raycastSuccess)
                {
                    _targetCubes.Add(hit.collider.transform);
                    if (hit.collider.CompareTag(CenterBlock))
                    {
                        _startRotation = hit.collider.transform.eulerAngles;
                        fakeParent.transform.position = hit.collider.transform.position;
                        fakeParent.transform.rotation = Quaternion.Euler(_startRotation);
                    }
                }
            }

            // Set the fake parent for rotation
            foreach (Transform targetCube in _targetCubes)
            {
                targetCube.SetParent(fakeParent.transform);
            }

            // Set the target rotation
            if (xIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.x = _cubeSideTargetRotation[index];
            }
            else if (yIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.y = _cubeSideTargetRotation[index];
            }
            else if (zIsNotZero)
            {
                _targetRotation = _startRotation;
                _targetRotation.z = _cubeSideTargetRotation[index];
            }

            _cubeTargetRotation[index] = _targetRotation;
        }

        private void UnParentCubeSides(int index)
        {

        }
    }

    #endregion

    #endregion

    #region Structs

    [System.Serializable]
    public struct CubeSide
    {
        [Header("Center")] public Transform centerCube;
        public Transform centerRayCastPosition;

        [Header("Direction and Distance")] public float raycastDistance;
        public Vector3 rayCastDirection;

        [Header("Adjacent Sides")] public List<int> adjacentSides;
    }

    [System.Serializable]
    public struct ParentSide
    {

    }

    #endregion
}