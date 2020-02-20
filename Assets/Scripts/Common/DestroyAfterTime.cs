using UnityEngine;

namespace Common
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destroyAfterTime = 3;

        private float m_currentTime;

        #region Unity Functions

        private void Start() => m_currentTime = destroyAfterTime;

        private void Update()
        {
            m_currentTime -= Time.deltaTime;
            if (m_currentTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}