using System;
using System.Collections.Generic;
using Audio;
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
        public LayerMask layerMask;

        [Header("Parent")] public Transform cubeParent;
        public float lerpSpeed;
        public float lerpEndingAmount;

        [Header("Player")] public PlayerGridController playerGridController;

        [Header("Audio")] public AudioController audioControl;

        [Header("Other Data")] public int rotationMultiplier = 9;

        private List<FakeParentData> _fakeParents;
        private List<SideRotationData> _sideRotations;

        private bool _ifGearTurnRang;

        #region Unity Functions

        private void Start()
        {
            _fakeParents = new List<FakeParentData>();
            _sideRotations = new List<SideRotationData>();

            for (int i = 0; i < cubeSides.Count; i++)
            {
                _sideRotations.Add(new SideRotationData()
                {
                    lerpAmount = 0,
                    currentSideRotation = 0,
                    targetSideRotation = 0,
                    startSideRotation = 0
                });

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
            UpdateParentRotations();
            UpdatePlayerMovementState();
        }

        #endregion

        #region External Functions

        public void CheckAndUpdateRotation(int sideIndex, int direction)
        {
            CubeSide cubeSide = cubeSides[sideIndex];
            bool isRotationPossible = true;

            for (int i = 0; i < cubeSide.adjacentSideIndexes.Count; i++)
            {
                int index = cubeSide.adjacentSideIndexes[i];
                float sideRotation = _sideRotations[index].currentSideRotation;

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

        #endregion

        #region Utility Functions

        #region Player Controls

        private void UpdatePlayerMovementState()
        {
            bool isMovementAllowed = true;
            for (int i = 0; i < _sideRotations.Count; i++)
            {
                float currentRotation = _sideRotations[i].currentSideRotation;
                if (currentRotation % RotationLocker != 0)
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
                SideRotationData sideRotation = _sideRotations[sideIndex];

                if (Math.Abs(sideRotation.currentSideRotation - sideRotation.targetSideRotation) % lerpSpeed < (lerpSpeed - 1) &&
                    _ifGearTurnRang)
                {
                    _ifGearTurnRang = false;
                }

                // Slowly increase lerp amount to update rotations
                float lerpAmount = sideRotation.lerpAmount + lerpSpeed * Time.deltaTime;
                sideRotation.lerpAmount = lerpAmount;
                sideRotation.currentSideRotation = Mathf.Lerp(
                    sideRotation.startSideRotation,
                    sideRotation.targetSideRotation,
                    lerpAmount
                );

                if (Math.Abs(sideRotation.currentSideRotation - sideRotation.targetSideRotation) % lerpSpeed >= (lerpSpeed - 1) &&
                    !_ifGearTurnRang)
                {
                    audioControl.PlaySound(AudioController.AudioEnum.GearTurning);
                    _ifGearTurnRang = true;
                }

                Vector3 currentRotation = fakeParentData.GetVectorRotation(sideRotation.currentSideRotation);
                fakeParentData.parent.transform.rotation = Quaternion.Euler(currentRotation);

                _fakeParents[i] = fakeParentData;

                int targetSideRotation = sideRotation.targetSideRotation;

                // UnParent the object when they reach the final rotation angle
                if (lerpAmount >= lerpEndingAmount && targetSideRotation % 90 == 0)
                {
                    audioControl.PlaySound(AudioController.AudioEnum.GearClicking);

                    sideRotation.currentSideRotation = sideRotation.targetSideRotation;
                    sideRotation.startSideRotation = sideRotation.targetSideRotation;
                    sideRotation.lerpAmount = 0;

                    Vector3 currentFinalRotation = fakeParentData.GetVectorRotation(sideRotation.currentSideRotation);
                    fakeParentData.parent.transform.rotation = Quaternion.Euler(currentFinalRotation);

                    _fakeParents.RemoveAt(i);

                    GameObject parent = fakeParentData.parent;
                    foreach (Transform childCube in fakeParentData.childCubes)
                    {
                        childCube.SetParent(cubeParent);
                    }

                    Destroy(parent);
                }

                _sideRotations[sideIndex] = sideRotation;
            }
        }

        #endregion

        #region Parent Control

        private void CheckAndCreateParent(int sideIndex, int direction)
        {
            SideRotationData sideRotationData = _sideRotations[sideIndex];
            sideRotationData.targetSideRotation += direction * rotationMultiplier;
            sideRotationData.startSideRotation = sideRotationData.currentSideRotation; // Update StartRotation as then the lerp can continue from the beginning
            sideRotationData.lerpAmount = 0; // Reset lerp amount to 0 to start from the starting
            _sideRotations[sideIndex] = sideRotationData;

            int currentRotation = (int) _sideRotations[sideIndex].currentSideRotation;
            int targetRotation = sideRotationData.targetSideRotation;

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
                    out RaycastHit hit, rayCastDistance,
                    layerMask
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
                targetRotation.x = _sideRotations[sideIndex].targetSideRotation;
            }
            else if (yIsNotZero)
            {
                targetRotation.y = _sideRotations[sideIndex].targetSideRotation;
            }
            else if (zIsNotZero)
            {
                targetRotation.z = _sideRotations[sideIndex].targetSideRotation;
            }

            FakeParentData fakeParentData = new FakeParentData()
            {
                childCubes = childCubes,
                parent = fakeParent,

                sideIndex = sideIndex,
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

        [Serializable]
        public struct CubeSide
        {
            [Header("Positions")] public Transform centerCube;
            public Transform rayCastPosition; // Can Be Removed and Calculated

            [Header("Direction and Distance")] public Vector3 rayCastDirection;
            public List<Transform> adjacentSideCenters;

            [Header("Internal Data")] public List<int> adjacentSideIndexes;
        }

        private struct SideRotationData
        {
            public float currentSideRotation;
            public float startSideRotation;
            public int targetSideRotation;
            public float lerpAmount;
        }

        private struct FakeParentData
        {
            public GameObject parent;
            public List<Transform> childCubes;

            public Vector3 targetRotationVector;
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