using UnityEngine;

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
        public float rotationOffsetStart;
        public Vector3 rotationOffsetAngle;

        [Header("Targetting")] public Transform cameraLookAt;
        public Transform mainCamera;

        private bool m_followPlayer;

        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private float m_lerpPositionAmount;

        private Vector3 m_startRotation;
        private Vector3 m_targetRotation;
        private float m_lerpRotationAmount;

        #region Unity Functions

        private void Start()
        {
            m_followPlayer = false;
            m_startPosition = cameraDefaultTransform.position;
            m_targetPosition = cameraDefaultTransform.position;
            m_lerpPositionAmount = 1;
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

            UpdateCameraRotation();
            UpdateCameraLookAt();
        }

        #endregion

        #region External Functions

        public void UpdateCameraRotation(Vector3 rotation)
        {
            float xValue = rotation.x + rotationOffsetAngle.x;
            float yValue = rotation.y + rotationOffsetAngle.y;
            float zValue = rotation.z + rotationOffsetAngle.z;

            if (Mathf.Abs(m_targetRotation.x - xValue) > rotationOffsetStart ||
                Mathf.Abs(m_targetRotation.y - yValue) > rotationOffsetStart ||
                Mathf.Abs(m_targetRotation.z - zValue) > rotationOffsetStart)
            {
                m_targetRotation = new Vector3(
                    xValue,
                    yValue,
                    zValue
                );
                m_startRotation = transform.rotation.eulerAngles;
                m_lerpRotationAmount = 0;
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
        }

        private void UpdateCameraLookAt() => mainCamera.LookAt(cameraLookAt);

        #endregion
    }
}