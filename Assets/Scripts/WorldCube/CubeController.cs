using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace WorldCube
{
    public class CubeController : MonoBehaviour
    {
        private const string CenterBlock = "CenterBlock";
        private const string ParentCube = "SideCubeParent";

        private const int RotationLocker = 90;

        [Header("Sides")] public List<CubeSide> cubeSides;
        public int sideSize;
        public float rayCastDistance;

        [Header("Parent")] public Transform cubeParent;
        public float lerpSpeed;
        public float minDifferenceBetweenAngles;

        [Header("Arduino")] public int readTimeout = 7;
        public int rotationMultiplier = 9;

        private List<FakeParentData> _fakeParents;
        private List<int> _sideTargetRotations;
        private List<int> _sideCurrentRotations;

        private SerialPort _serialPort;
        private string _lastInputType;

        #region Unity Functions

        private void Start()
        {
            _fakeParents = new List<FakeParentData>();
            _sideTargetRotations = new List<int>();
            _sideCurrentRotations = new List<int>();

            _serialPort = new SerialPort("COM4", 9600);
            _serialPort.ReadTimeout = readTimeout;
            if (_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            for (int i = 0; i < cubeSides.Count; i++)
            {
                _sideCurrentRotations.Add(0);
                _sideTargetRotations.Add(0);
            }
        }

        private void Update()
        {
            ReadInput();
            UpdateParentRotations();
        }

        #endregion

        #region Arduino

        private void ReadInput()
        {
            string input = _serialPort.ReadLine();
            switch (input)
            {
                case "A":
                case "B":
                    _lastInputType = input;
                    break;

                default:
                {
                    int direction = int.Parse(input);
                    switch (_lastInputType)
                    {
                        case "A":
                            CheckAndUpdateRotation(0, direction);
                            break;

                        case "B":
                            CheckAndUpdateRotation(1, direction);
                            break;

                        default:
                            Debug.Log("Invalid Input Sent");
                            break;
                    }
                }
                    break;
            }
        }

        #endregion

        #region Utility Functions

        #region Parent Updates

        private void UpdateParentRotations()
        {
            for (var i = 0; i < _fakeParents.Count; i++)
            {
                FakeParentData fakeParentData = _fakeParents[i];

                int sideIndex = fakeParentData.sideIndex;
                _sideCurrentRotations[sideIndex] =
                    (int) Mathf.Lerp(_sideCurrentRotations[sideIndex], _sideTargetRotations[sideIndex], lerpSpeed * Time.deltaTime);

                Vector3 currentRotation = fakeParentData.GetVectorRotation(_sideCurrentRotations[sideIndex]);
                fakeParentData.parent.transform.rotation = Quaternion.Euler(currentRotation);

                _fakeParents[i] = fakeParentData;

                // UnParent the object when they reach the final rotation angle
                if (Mathf.Abs(_sideCurrentRotations[sideIndex] - _sideTargetRotations[sideIndex]) <= minDifferenceBetweenAngles)
                {
                    _sideCurrentRotations[sideIndex] = _sideTargetRotations[sideIndex];
                    Vector3 currentFinalRotation = fakeParentData.GetVectorRotation(_sideCurrentRotations[sideIndex]);
                    fakeParentData.parent.transform.rotation = Quaternion.Euler(currentFinalRotation);

                    _fakeParents.RemoveAt(i);

                    GameObject parent = fakeParentData.parent;
                    foreach (Transform childCube in fakeParentData.childCubes)
                    {
                        childCube.SetParent(cubeParent);
                    }

                    Destroy(parent);
                }
            }
        }

        #endregion

        #region Parent Control

        private void CheckAndUpdateRotation(int sideIndex, int direction)
        {
            CubeSide cubeSide = cubeSides[sideIndex];
            bool isRotationPossible = true;

            for (int i = 0; i < cubeSide.adjacentSideIndexes.Count; i++)
            {
                int index = cubeSide.adjacentSideIndexes[i];
                int sideRotation = _sideCurrentRotations[index];

                if (sideRotation % RotationLocker != 0)
                {
                    isRotationPossible = false;
                    break;
                }
            }

            if (isRotationPossible)
            {
                ParentSetter(sideIndex, direction);
            }
        }

        private void ParentSetter(int sideIndex, int direction)
        {
            _sideTargetRotations[sideIndex] += direction * rotationMultiplier;

            int currentRotation = _sideCurrentRotations[sideIndex];
            int targetRotation = _sideTargetRotations[sideIndex];

            // This is the case when the object needs to parented
            if (currentRotation % RotationLocker == 0 && targetRotation % RotationLocker != 0)
            {
                CreateAndSaveParent(sideIndex);
            }

            // Just increment the target rotation
            else if (currentRotation % RotationLocker != 0)
            {
                var targetData = GetTargetParent(sideIndex);
                FakeParentData fakeParentData = targetData.Item1;
                fakeParentData.targetRotation = _sideTargetRotations[sideIndex];
                _fakeParents[targetData.Item2] = fakeParentData;
            }
        }

        private void CreateAndSaveParent(int sideIndex)
        {
            CubeSide cubeSide = cubeSides[sideIndex];
            Vector3 rayCastPosition = cubeSide.rayCastPosition.position;
            Vector3 direction = cubeSide.rayCastDirection;

            bool xIsNotZero = direction.x != 0;
            bool yIsNotZero = direction.y != 0;
            bool zIsNotZero = direction.z != 0;

            // Find all the positions from where the rayCasts will fire
            List<Vector3> positions = new List<Vector3>();
            for (int i = -sideSize; i <= sideSize; i++)
            {
                for (int j = -sideSize; i <= sideSize; j++)
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

                    Vector3 finalPosition = rayCastPosition + new Vector3(xValue, yValue, zValue);
                    positions.Add(finalPosition);
                }
            }

            List<Transform> childCubes = new List<Transform>();
            GameObject fakeParent = new GameObject();
            Vector3 startRotation = Vector3.zero;

            // Find all cubes hit by rayCast
            foreach (Vector3 position in positions)
            {
                bool raycastSuccess = Physics.Raycast(
                    position, cubeSide.rayCastDirection,
                    out RaycastHit hit, rayCastDistance
                );
                if (raycastSuccess)
                {
                    childCubes.Add(hit.collider.transform);
                    if (hit.collider.CompareTag(CenterBlock))
                    {
                        startRotation = hit.collider.transform.parent.eulerAngles;
                        fakeParent.transform.position = hit.collider.transform.position;
                        fakeParent.transform.rotation = Quaternion.Euler(startRotation);
                    }
                }
            }

            // Set the fake parent for rotation
            foreach (Transform targetCube in childCubes)
            {
                targetCube.SetParent(fakeParent.transform);
            }

            Vector3 targetRotation = startRotation;
            // Set the target rotation
            if (xIsNotZero)
            {
                targetRotation.x = _sideTargetRotations[sideIndex];
            }
            else if (yIsNotZero)
            {
                targetRotation.y = _sideTargetRotations[sideIndex];
            }
            else if (zIsNotZero)
            {
                targetRotation.z = _sideTargetRotations[sideIndex];
            }

            FakeParentData fakeParentData = new FakeParentData()
            {
                childCubes = childCubes,
                parent = fakeParent,

                sideIndex = sideIndex,
                targetRotation = _sideTargetRotations[sideIndex],
                targetRotationVector = targetRotation
            };
            _fakeParents.Add(fakeParentData);
        }

        private (FakeParentData, int) GetTargetParent(int sideIndex)
        {
            for (var i = 0; i < _fakeParents.Count; i++)
            {
                FakeParentData fakeParentData = _fakeParents[i];
                if (fakeParentData.sideIndex == sideIndex)
                {
                    return (fakeParentData, i);
                }
            }

            throw new Exception("Invalid Index Requested");
        }

        #endregion

        #endregion

        #region Structs

        [System.Serializable]
        public struct CubeSide
        {
            [Header("Positions")] public Transform centerCube;
            public Transform rayCastPosition; // Can Be Removed and Calculated

            [Header("Direction and Distance")] public Vector3 rayCastDirection;
            public List<int> adjacentSideIndexes;
        }

        private struct FakeParentData
        {
            public GameObject parent;
            public List<Transform> childCubes;

            public Vector3 targetRotationVector;
            public float targetRotation;
            public int sideIndex;

            public bool targetReached;

            public Vector3 GetVectorRotation(int rotation)
            {
                // TODO: Complete this function
                return Vector3.zero;
            }
        }

        #endregion
    }
}