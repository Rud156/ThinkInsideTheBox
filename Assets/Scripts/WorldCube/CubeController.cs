using System;
using System.Collections.Generic;
using System.IO.Ports;
using Player;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeController : MonoBehaviour
    {
        private const int RotationLocker = 90;

        [Header("Sides")] public List<CubeSide> cubeSides;
        public int sideSize;
        public float rayCastDistance;

        [Header("Parent")] public Transform cubeParent;
        public float lerpSpeed;
        public float minDifferenceBetweenAngles;

        [Header("Player")] public PlayerGridController playerGridController;

        [Header("Arduino")] public int readTimeout = 7;
        public int rotationMultiplier = 9;
        public bool useForcedPort = false;
        public string portString = "COM3";
        public bool disableSerialPort;

        private List<FakeParentData> _fakeParents;
        private List<float> _sideTargetRotations;
        private List<float> _sideCurrentRotations;

        private SerialPort _serialPort;

        #region Unity Functions

        private void Start()
        {
            _fakeParents = new List<FakeParentData>();
            _sideTargetRotations = new List<float>();
            _sideCurrentRotations = new List<float>();

            string[] ports = SerialPort.GetPortNames();
            string portName;
            if (disableSerialPort)
            {
                portName = portString;
            }
            else
            {
                portName = ports[0]; // TODO: Use ManagementObject to find the data regarding the port
            }

            if (useForcedPort)
            {
                portName = portString;
            }

            Debug.Log($"Target Port: {portName}");

            _serialPort = new SerialPort(portName, 9600);
            _serialPort.ReadTimeout = readTimeout;
            if (!_serialPort.IsOpen)
            {
                if (!disableSerialPort)
                {
                    _serialPort.Open();
                }

                Debug.Log("Port is Closed. Opening");
            }

            for (int i = 0; i < cubeSides.Count; i++)
            {
                _sideCurrentRotations.Add(0);
                _sideTargetRotations.Add(0);

                CubeSide cubeSide = cubeSides[i];
                cubeSide.adjacentSideIndexes = new List<int>();
                for (int j = 0; j < cubeSide.adjacentSideCenters.Count; j++)
                {
                    int sideIndex = GetAdjacentSideIndex(cubeSide.adjacentSideCenters[j]);
                    if (sideIndex != -1)
                    {
                        cubeSide.adjacentSideIndexes.Add(sideIndex);
                    }
                }

                cubeSides[i] = cubeSide;
            }
        }

        private void Update()
        {
            ReadInput();
            UpdateParentRotations();
            UpdatePlayerMovementState();

            HandleKeyboardInput();
        }

        private void OnApplicationQuit() => _serialPort.Close();

        #endregion

        #region Testing

        private void HandleKeyboardInput()
        {
            if (playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CheckAndUpdateRotation(0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                CheckAndUpdateRotation(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CheckAndUpdateRotation(1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                CheckAndUpdateRotation(1, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                CheckAndUpdateRotation(2, 1);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                CheckAndUpdateRotation(2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                CheckAndUpdateRotation(3, 1);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                CheckAndUpdateRotation(3, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                CheckAndUpdateRotation(4, 1);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                CheckAndUpdateRotation(4, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                CheckAndUpdateRotation(5, 1);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                CheckAndUpdateRotation(5, -1);
            }
        }

        #endregion

        #region Arduino

        private void ReadInput()
        {
            if (playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            try
            {
                string input = _serialPort.ReadLine();
                Debug.Log(input);

                string[] splitInput = input.Split(':');
                string sideInput = splitInput[0];
                string directionString = splitInput[1];

                switch (input)
                {
                    case "Left":
                    case "Right":
                    case "Front":
                    case "Back":
                    case "Top":
                        sideInput = input;
                        break;

                    default:
                    {
                        bool parseSuccess = int.TryParse(directionString, out int direction);
                        if (!parseSuccess)
                        {
                            return;
                        }

                        switch (sideInput)
                        {
                            case "Left":
                                CheckAndUpdateRotation(1, direction);
                                break;

                            case "Right":
                                CheckAndUpdateRotation(3, direction);
                                break;

                            case "Front":
                                CheckAndUpdateRotation(2, direction);
                                break;

                            case "Back":
                                CheckAndUpdateRotation(0, direction);
                                break;

                            case "Top":
                                CheckAndUpdateRotation(4, direction);
                                break;

                            default:
                                Debug.Log("Invalid Input Sent");
                                break;
                        }
                    }
                        break;
                }
            }
            catch (TimeoutException te)
            {
                // Don't do anything. This is not required as there is no input
            }
            catch (InvalidOperationException ioe)
            {
                // Don't do anything. This is not required as there is nothing connected
            }
        }

        #endregion

        #region Utility Functions

        #region Player Controls

        private void UpdatePlayerMovementState()
        {
            bool isMovementAllowed = true;
            for (int i = 0; i < _sideCurrentRotations.Count; i++)
            {
                if (_sideCurrentRotations[i] % 90 != 0)
                {
                    isMovementAllowed = false;
                    break;
                }
            }

            if (isMovementAllowed)
            {
                playerGridController.AllowPlayerMovement();
            }
            else
            {
                playerGridController.PreventPlayerMovement();
            }
        }

        #endregion

        #region Parent Updates

        private void UpdateParentRotations()
        {
            for (var i = 0; i < _fakeParents.Count; i++)
            {
                FakeParentData fakeParentData = _fakeParents[i];

                int sideIndex = fakeParentData.sideIndex;
                _sideCurrentRotations[sideIndex] =
                    (int) Mathf.Lerp(_sideCurrentRotations[sideIndex], _sideTargetRotations[sideIndex],
                        lerpSpeed * Time.deltaTime);

                Vector3 currentRotation = fakeParentData.GetVectorRotation(_sideCurrentRotations[sideIndex]);
                fakeParentData.parent.transform.rotation = Quaternion.Euler(currentRotation);

                _fakeParents[i] = fakeParentData;

                int targetSideRotation = (int) _sideTargetRotations[sideIndex];

                // UnParent the object when they reach the final rotation angle
                if (Mathf.Abs(_sideCurrentRotations[sideIndex] - _sideTargetRotations[sideIndex]) <=
                    minDifferenceBetweenAngles && targetSideRotation % 90 == 0)
                {
                    //FindObjectOfType<AudioController>().Play("GearClicking");
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
                int sideRotation = (int) _sideCurrentRotations[index];

                if (sideRotation % RotationLocker != 0)
                {
                    isRotationPossible = false;
                    break;
                }
            }

            if (isRotationPossible)
            {
                CheckAndCreateParent(sideIndex, direction);
            }
        }

        private void CheckAndCreateParent(int sideIndex, int direction)
        {
            _sideTargetRotations[sideIndex] += direction * rotationMultiplier;

            int currentRotation = (int) _sideCurrentRotations[sideIndex];
            int targetRotation = (int) _sideTargetRotations[sideIndex];

            // This is the case when the object needs to parented
            if (currentRotation % RotationLocker == 0 && targetRotation % RotationLocker != 0 &&
                !SideHasParent(sideIndex))
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
            for (int i = -sideSize; i <= sideSize; i += sideSize)
            {
                for (int j = -sideSize; j <= sideSize; j += sideSize)
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
                    childCubes.Add(hit.collider.transform.parent);
                    if (hit.collider.CompareTag(TagManager.CenterBlock))
                    {
                        startRotation = hit.collider.transform.parent.eulerAngles;
                        Transform centerChild = GetCenterMarker(hit.collider.transform.parent);
                        fakeParent.transform.position = centerChild.position;
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
                targetRotationVector = targetRotation,

                xIsNotZero = xIsNotZero,
                yIsNotZero = yIsNotZero,
                zIsNotZero = zIsNotZero
            };
            _fakeParents.Add(fakeParentData);
        }

        private (FakeParentData, int) GetTargetParent(int sideIndex)
        {
            for (int i = 0; i < _fakeParents.Count; i++)
            {
                FakeParentData fakeParentData = _fakeParents[i];
                if (fakeParentData.sideIndex == sideIndex)
                {
                    return (fakeParentData, i);
                }
            }

            throw new Exception("Invalid Index Requested");
        }

        private bool SideHasParent(int sideIndex)
        {
            for (int i = 0; i < _fakeParents.Count; i++)
            {
                if (_fakeParents[i].sideIndex == sideIndex)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Index Utils

        private int GetAdjacentSideIndex(Transform centerTransform)
        {
            for (int i = 0; i < cubeSides.Count; i++)
            {
                if (cubeSides[i].centerCube == centerTransform)
                {
                    return i;
                }
            }

            return -1;
        }

        private Transform GetCenterMarker(Transform sideParent)
        {
            for (int i = 0; i < sideParent.childCount; i++)
            {
                Transform child = sideParent.GetChild(i);
                if (child.CompareTag(TagManager.SideCenterMarker))
                {
                    return child;
                }
            }

            throw new Exception("Invalid Parent Requested");
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
            public List<Transform> adjacentSideCenters;

            [Header("Internal Data")] public List<int> adjacentSideIndexes;
        }

        private struct FakeParentData
        {
            public GameObject parent;
            public List<Transform> childCubes;

            public Vector3 targetRotationVector;
            public float targetRotation;
            public int sideIndex;

            public bool xIsNotZero;
            public bool yIsNotZero;
            public bool zIsNotZero;

            public Vector3 GetVectorRotation(float rotation)
            {
                Vector3 vectorRotation = Vector3.zero;

                if (xIsNotZero)
                {
                    vectorRotation.x = rotation;
                }
                else if (yIsNotZero)
                {
                    vectorRotation.y = rotation;
                }
                else if (zIsNotZero)
                {
                    vectorRotation.z = rotation;
                }

                return vectorRotation;
            }
        }

        #endregion
    }
}