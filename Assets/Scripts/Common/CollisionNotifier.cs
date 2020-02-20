using UnityEngine;

namespace Common
{
    public class CollisionNotifier : MonoBehaviour
    {
        public delegate void TriggerEnterNotifier(Collider other);
        public delegate void TriggerExitNotifier(Collider other);

        public TriggerEnterNotifier OnTriggerEnterNotifier;
        public TriggerExitNotifier OnTriggerExitNotifier;

        #region Unity Functions

        private void OnTriggerEnter(Collider other) => OnTriggerEnterNotifier?.Invoke(other);

        private void OnTriggerExit(Collider other) => OnTriggerExitNotifier?.Invoke(other);

        #endregion
    }
}