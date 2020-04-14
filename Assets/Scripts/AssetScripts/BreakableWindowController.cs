using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace AssetScripts
{
    public class BreakableWindowController : MonoBehaviour
    {
        public PlayableDirector playableDirector;
        public MeshRenderer destroyMesh;
        public float animationTime;

        private float m_currentTime;
        private bool m_animationActive;

        #region Unity Functions

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
            {
                m_currentTime = animationTime;
                m_animationActive = true;
                playableDirector.Play();
            }
        }

        private void Update()
        {
            if (!m_animationActive)
            {
                return;
            }

            m_currentTime -= Time.deltaTime;
            if (m_currentTime <= 0)
            {
                destroyMesh.enabled = false;
            }
        }

        #endregion
    }
}