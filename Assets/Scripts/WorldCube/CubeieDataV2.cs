using System;
using UnityEngine;

namespace WorldCube
{
    public class CubeieDataV2 : MonoBehaviour
    {
        private bool m_hasParent;
        private CubeLayerMaskV2 m_currentLayerMask;
        private Transform m_parentTransform;
        private Transform m_originalParent;

        #region Unity Functions

        private void Start() => m_originalParent = transform.parent;

        #endregion

        #region External Functions

        public void SetParent(CubeLayerMaskV2 iCubeLayerMask, Transform i_parentTransform)
        {
            if (m_hasParent)
            {
                throw new Exception("Trying to access cube that already has a parent");
            }

            m_hasParent = true;
            m_currentLayerMask = iCubeLayerMask;
            m_parentTransform = i_parentTransform;

            transform.SetParent(i_parentTransform);
        }

        public void ReleaseParent(CubeLayerMaskV2 i_cubeLayerMask)
        {
            if (i_cubeLayerMask != m_currentLayerMask)
            {
                throw new Exception("Invalid layer is trying to release");
            }

            m_hasParent = false;
            m_currentLayerMask = null;
            m_parentTransform = null;

            transform.SetParent(m_originalParent);
        }

        public bool HasParent => m_hasParent;

        #endregion
    }
}