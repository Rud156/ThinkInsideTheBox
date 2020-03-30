using System;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeSideTransparencyControl : MonoBehaviour
    {
        private const string Alpha = "_Alpha";

        [Header("Change Rates")] public float transparencyChangeRate;
        public float minTransparency = 0;
        public float maxTransparency = 1;

        [Header("Collider Detection")] public float sphereRadius = 1;
        public LayerMask layerMask;

        private Material m_materialInstance;

        private float m_startTransparency;
        private float m_targetTransparency;
        private float m_lerpAmount;

        private bool m_lerpActive;

        #region Unity Functions

        private void Update()
        {
            GrabSideMaterials();

            if (!m_lerpActive)
            {
                return;
            }

            m_lerpAmount += transparencyChangeRate * Time.deltaTime;
            float currentValue = Mathf.Lerp(m_startTransparency, m_targetTransparency, m_lerpAmount);
            m_materialInstance.SetFloat(Alpha, currentValue);

            if (m_lerpAmount > 0.97f)
            {
                m_materialInstance.SetFloat(Alpha, m_targetTransparency);
                m_lerpActive = false;
            }
        }

        #endregion

        #region External Functions

        public void MakeSideVisible()
        {
            float currentAlpha = m_materialInstance.GetFloat(Alpha);

            m_startTransparency = currentAlpha;
            m_targetTransparency = minTransparency;
            m_lerpAmount = 0;
            m_lerpActive = true;
        }

        public void MakeSideTransparent()
        {
            float currentAlpha = m_materialInstance.GetFloat(Alpha);

            m_startTransparency = currentAlpha;
            m_targetTransparency = maxTransparency;
            m_lerpAmount = 0;
            m_lerpActive = true;
        }

        #endregion

        #region Utility Functions

        private void GrabSideMaterials()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, layerMask);
            foreach (Collider other in colliders)
            {
                if (other.CompareTag(TagManager.FaceOut) || other.CompareTag(TagManager.WaterHole))
                {
                    Material[] materials = other.GetComponent<Renderer>().materials;
                    if (materials.Length < 2)
                    {
                        continue;
                    }

                    if (m_materialInstance != null && materials[1] != m_materialInstance)
                    {
                        m_materialInstance.SetFloat(Alpha, 1);
                    }

                    m_materialInstance = materials[1];
                    break;
                }
            }
        }

        #endregion
    }
}