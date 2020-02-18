using UnityEngine;
using Utils;

namespace WorldCube
{
    public class WorldCubeShaderController : MonoBehaviour
    {
        private const string IsHighlighted = "IsHighlighted";

        #region Unity Functions

        private Material m_cubeMaterial;

        private void Start()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            m_cubeMaterial = meshRenderer.material;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.WorldCubeMarker))
            {
                m_cubeMaterial.SetInt(IsHighlighted, 1);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.WorldCubeMarker))
            {
                m_cubeMaterial.SetInt(IsHighlighted, 0);
            }
        }

        #endregion
    }
}