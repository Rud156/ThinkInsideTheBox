using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Utils;
using WorldCube;

namespace CustomCamera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Positions")] public Transform cameraDefaultTransform;
        public Vector3 playerFollowOffset;
        public Transform playerTransform;

        [Header("Position Lerping")] public float positionLerpSpeed;
        public AnimationCurve positionLerpCurve;

        [Header("Rotation Lerping")] public float rotationLerpSpeed;
        public AnimationCurve rotationLerpCurve;
        public Vector3 rotationOffsetAngle;

        [Header("Rotation Normalization")] public int rotationGroupAmount;
        public float rotationOffsetTolerance;
        public bool useRotationNormalization;
        public CubeRotationHandler cubeRotationHandler;

        [Header("Manual Camera Control")] public float manualCameraRotationSpeed;

        [Header("Targeting")] public Transform cameraLookAt;
        public Transform mainCamera;

        private bool m_followPlayer;

        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private float m_lerpPositionAmount;

        private Vector3 m_startRotation;
        private Vector3 m_targetRotation;
        private float m_lerpRotationAmount;

        private List<Vector3> m_previousRotations;

        #region Unity Functions

        private void Start()
        {
            m_followPlayer = false;
            m_startPosition = cameraDefaultTransform.position;
            m_targetPosition = cameraDefaultTransform.position;
            m_lerpPositionAmount = 1;

            m_previousRotations = new List<Vector3>();
        }

        private void Update()
        {
            // if (m_followPlayer)
            // {
            //     Vector3 playerPosition = playerTransform.position + playerFollowOffset;
            //     if (m_targetPosition != playerPosition)
            //     {
            //         m_targetPosition = playerPosition;
            //         m_startPosition = transform.position;
            //         m_lerpPositionAmount = 0;
            //     }
            // }
            //
            // if (m_lerpPositionAmount < 1)
            // {
            //     m_lerpPositionAmount += positionLerpSpeed * Time.deltaTime;
            // }
            //
            // transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, positionLerpCurve.Evaluate(m_lerpPositionAmount));

            UpdateManualCameraMovement();
            UpdateCameraRotation();
            // UpdateCameraLookAt();
        }

        #endregion

        #region External Functions

        public void SetTargetCameraRotation(Vector3 i_rotation)
        {
            if (useRotationNormalization)
            {
                SetTargetRotationWithNormalization(i_rotation);
            }
            else
            {
                SetTargetRotationWithoutNormalization(i_rotation);
            }
        }

        public void SetCameraDefaultPosition()
        {
            m_targetPosition = cameraDefaultTransform.position;
            m_startPosition = transform.position;
            m_lerpPositionAmount = 0;
        }

        public void SetFollowActive() => m_followPlayer = true;

        public void DeactivateFollow() => m_followPlayer = false;

        #endregion

        #region Utility Functions

        private void UpdateManualCameraMovement()
        {
            if (Input.GetKey(ControlConstants.CameraLeft))
            {
                m_targetRotation.y -= manualCameraRotationSpeed * Time.deltaTime;
                m_startRotation = transform.rotation.eulerAngles;
                m_lerpRotationAmount = 0;
            }
            else if (Input.GetKey(ControlConstants.CameraRight))
            {
                m_targetRotation.y += manualCameraRotationSpeed * Time.deltaTime;
                m_startRotation = transform.rotation.eulerAngles;
                m_lerpRotationAmount = 0;
            }
        }

        private void UpdateCameraRotation()
        {
            if (m_lerpRotationAmount < 1)
            {
                m_lerpRotationAmount += rotationLerpSpeed * Time.deltaTime;
            }

            // Not Optimized. But works
            Quaternion start = Quaternion.Euler(m_startRotation);
            Quaternion target = Quaternion.Euler(m_targetRotation);
            transform.rotation = Quaternion.Slerp(start, target, rotationLerpCurve.Evaluate(m_lerpRotationAmount));

            cubeRotationHandler.RotateCenter(transform.rotation.eulerAngles.y);
        }

        private void UpdateCameraLookAt() => mainCamera.LookAt(cameraLookAt);

        private void SetTargetRotationWithNormalization(Vector3 i_rotation)
        {
            float zValue = i_rotation.x + rotationOffsetAngle.x;
            float yValue = i_rotation.y + rotationOffsetAngle.y;
            float xValue = i_rotation.z + rotationOffsetAngle.z;

            m_previousRotations.Add(new Vector3(xValue, yValue, zValue));
            if (m_previousRotations.Count > rotationGroupAmount)
            {
                // Pretty sure there is a better way to do this...

                int differentCheckCount = 0;
                for (int i = 1; i < m_previousRotations.Count; i++)
                {
                    // Check for Single Difference
                    if (!ExtensionFunctions.IsSimilarVector(m_previousRotations[0], m_previousRotations[i], rotationOffsetTolerance))
                    {
                        differentCheckCount += 1;
                    }
                }

                // Only a single wrong value crept in...
                if (differentCheckCount == 1)
                {
                    m_targetRotation = m_previousRotations[0];
                }
                else
                {
                    differentCheckCount = 0;
                    for (int i = 0; i < m_previousRotations.Count; i++)
                    {
                        if (!ExtensionFunctions.IsSimilarVector(m_previousRotations[1], m_previousRotations[i], rotationOffsetTolerance))
                        {
                            differentCheckCount += 1;
                        }
                    }

                    if (differentCheckCount == 1)
                    {
                        m_targetRotation = m_previousRotations[1];
                    }
                    else
                    {
                        m_targetRotation = m_previousRotations[m_previousRotations.Count - 1];
                    }
                }


                m_startRotation = transform.rotation.eulerAngles;
                m_lerpRotationAmount = 0;

                m_previousRotations.Clear();
            }
        }

        private void SetTargetRotationWithoutNormalization(Vector3 i_rotation)
        {
            float zValue = i_rotation.x + rotationOffsetAngle.x;
            float yValue = i_rotation.y + rotationOffsetAngle.y;
            float xValue = i_rotation.z + rotationOffsetAngle.z;

            Vector3 newRotation = new Vector3(xValue, yValue, zValue);
            if (!ExtensionFunctions.IsSimilarVector(m_targetRotation, newRotation))
            {
                m_targetRotation = newRotation;
                m_startRotation = transform.rotation.eulerAngles;
                m_lerpRotationAmount = 0;
            }
        }

        #endregion
    }
}