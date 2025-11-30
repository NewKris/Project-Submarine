using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Interaction {
    public class InteractionController : MonoBehaviour {
        public float range;
        public LayerMask interactionMask;
        public GameObject prompt;

        private Interactable _interactable;
        
        public void TryInteract() {
            if (_interactable) {
                _interactable.Interact();
            }
        }

        private void Update() {
            _interactable = FindInteraction();
        }

        private Interactable FindInteraction() {
            Ray ray = new Ray(transform.position, transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, interactionMask)) {
                prompt.SetActive(true);
                return hit.collider.GetComponent<Interactable>();
            }

            prompt.SetActive(false);
            return null;
        }
        
        private void OnDrawGizmos() {
            HandlesProxy.DrawRay(transform.position, transform.forward * range, 3, false, Color.red);
        }
    }
}