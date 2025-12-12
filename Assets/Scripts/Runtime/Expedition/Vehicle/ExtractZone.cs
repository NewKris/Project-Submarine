using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Vehicle {
    public class ExtractZone : MonoBehaviour {
        public UnityEvent<bool> onOverlapChanged;

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Submarine")) {
                onOverlapChanged.Invoke(true);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Submarine")) {
                onOverlapChanged.Invoke(false);
            }
        }
    }
}