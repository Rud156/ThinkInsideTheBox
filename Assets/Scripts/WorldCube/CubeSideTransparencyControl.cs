using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace WorldCube
{
    public class CubeSideTransparencyControl : MonoBehaviour
    {
        private const string Alpha = "_Alpha";
        private const float TransparentAlpha = 1f;
        private const float OpaqueAlpha = 0.3f;

        public float transparencyChangeRate = 7;

        private List<MaterialTransparency> m_materialTransparency;

        #region Unity Functions

        private void Start() => m_materialTransparency = new List<MaterialTransparency>();

        private void Update()
        {
            for (int i = m_materialTransparency.Count - 1; i >= 0; i--)
            {
                MaterialTransparency materialTransparency = m_materialTransparency[i];
                materialTransparency.UpdateMaterial(transparencyChangeRate);

                m_materialTransparency[i] = materialTransparency;

                if (materialTransparency.lerpAmount == 1 && materialTransparency.isLerpTowardsTransparent)
                {
                    m_materialTransparency.RemoveAt(i);
                }
            }
        }

        #endregion

        #region External Functions

        public void MakeSideVisible(Transform i_targetSide)
        {
            bool didFindMaterial = GrabSideMaterials(i_targetSide, out MaterialTransparency materialTransparency, out int index);
            if (didFindMaterial)
            {
                materialTransparency.MakeVisible();
                m_materialTransparency[index] = materialTransparency;
            }
        }

        public void MakeSideTransparent(Transform i_targetSide)
        {
            bool didFindMaterial = GrabSideMaterials(i_targetSide, out MaterialTransparency materialTransparency, out int index);
            if (didFindMaterial)
            {
                materialTransparency.MakeTransparent();
                m_materialTransparency[index] = materialTransparency;
            }
        }

        #endregion

        #region Utility Functions

        private bool GrabSideMaterials(Transform i_transform, out MaterialTransparency o_materialTransparency, out int o_index)
        {
            for (var i = 0; i < m_materialTransparency.Count; i++)
            {
                MaterialTransparency transparency = m_materialTransparency[i];
                if (transparency.transform == i_transform)
                {
                    o_materialTransparency = transparency;
                    o_index = i;
                    return true;
                }
            }

            if (i_transform.CompareTag(TagManager.FaceOut) || i_transform.CompareTag(TagManager.WaterHole))
            {
                Material material;

                if (i_transform.CompareTag(TagManager.FaceOut))
                {
                    material = i_transform.GetComponent<Renderer>().materials[0];
                }
                else
                {
                    material = i_transform.GetComponent<Renderer>().materials[0];
                }

                MaterialTransparency transparency = new MaterialTransparency()
                {
                    transform = i_transform,
                    material = material,

                    lerpAmount = 2,
                    startTransparency = material.GetFloat(Alpha),
                    targetTransparency = CubeSideTransparencyControl.TransparentAlpha
                };

                o_materialTransparency = transparency;
                o_index = m_materialTransparency.Count;
                m_materialTransparency.Add(transparency); // Add to list and then return
                return true;
            }

            o_materialTransparency = new MaterialTransparency();
            o_index = -1;
            return false;
        }

        #endregion

        #region Structs

        private struct MaterialTransparency
        {
            public Transform transform;
            public Material material;

            public float startTransparency;
            public float targetTransparency;
            public float lerpAmount;

            public bool isLerpTowardsTransparent;

            public void MakeTransparent()
            {
                lerpAmount = 0;
                startTransparency = material.GetFloat(Alpha);
                targetTransparency = CubeSideTransparencyControl.TransparentAlpha;
                isLerpTowardsTransparent = true;
            }

            public void MakeVisible()
            {
                lerpAmount = 0;
                startTransparency = material.GetFloat(Alpha);
                targetTransparency = CubeSideTransparencyControl.OpaqueAlpha;
                isLerpTowardsTransparent = false;
            }

            public void UpdateMaterial(float i_alphaChangeRate)
            {
                if (lerpAmount >= 1)
                {
                    return;
                }

                lerpAmount += i_alphaChangeRate * Time.deltaTime;
                if (lerpAmount >= 0.97f)
                {
                    material.SetFloat(Alpha, targetTransparency);
                    lerpAmount = 1;
                }
                else
                {
                    material.SetFloat(Alpha, Mathf.Lerp(startTransparency, targetTransparency, lerpAmount));
                }
            }
        }

        #endregion
    }
}