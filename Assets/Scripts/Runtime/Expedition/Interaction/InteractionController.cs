using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction.Interface;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Interaction {
    public class InteractionController : MonoBehaviour {
        public float range;
        public LayerMask interactionMask;
        public GameObject prompt;

        private Interactable _interactable;
        private InterfaceControl _activeControl;
        
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
        
        private void TryGrabHandle() {
            Ray ray = Camera.main.ScreenPointToRay(PlayerInputListener.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                InterfaceControl control = hit.collider.GetComponentInParent<InterfaceControl>();
                _activeControl = control;
                _activeControl?.OnHandleStart();
            }
        }

        private void ReleaseHandle() {
            if (_activeControl) {
                _activeControl?.OnHandleStop();
                _activeControl = null;
            }
        }
    }
}