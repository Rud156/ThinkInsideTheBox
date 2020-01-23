using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destroyAfterTime = 3;

        private float _currentTime;

        #region Unity Functions

        private void Start() => _currentTime = destroyAfterTime;

        private void Update()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}