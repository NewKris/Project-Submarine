using UnityEngine;
using WereHorse.Runtime.Utility.CommonObjects;

namespace WereHorse.Runtime.Utility.CommonBehaviours {
    /// <summary>
    /// Follows a target with soft damping.
    /// </summary>
    public class Drone : MonoBehaviour {
        public Transform target;
        public float damping;

        private DampedVector _position;

        private void Awake() {
            Vector3 startPosition = target ? target.position : transform.position;
            _position = new DampedVector(startPosition);
        }

        private void LateUpdate() {
            if (!target) {
                return;
            }
            
            _position.Target = target.position;
            transform.position = _position.Tick(damping);
        }
    }
}
