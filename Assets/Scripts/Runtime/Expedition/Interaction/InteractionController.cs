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
        private InterfaceControl _grabbedControl;
        
        public void TryInteract() {
            if (_interactable) {
                _interactable.Interact();
            }
        }
        
        public bool TryGrabHandle(out InterfaceControl control) {
            Ray ray = Camera.main.ScreenPointToRay(PlayerInputListener.MousePosition);
            control = null;
            
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                control = hit.collider.GetComponentInParent<InterfaceControl>();
                _grabbedControl = control;
                _grabbedControl?.OnHandleStart();
            }

            return control != null;
        }
        
        public bool TryReleaseHandle() {
            if (_grabbedControl) {
                _grabbedControl?.OnHandleStop();
                _grabbedControl = null;
                return true;
            }

            return false;
        }

        private void Update() {
            _interactable = FindInteraction();
        }

        private Interactable FindInteraction() {
            Ray ray = new Ray(transform.position, transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, interactionMask) 
                && hit.collider.TryGetComponent(out Interactable interactable)
            ) {
                prompt.SetActive(true);
                return interactable;
            }

            prompt.SetActive(false);
            return null;
        }
        
        private void OnDrawGizmos() {
            HandlesProxy.DrawRay(transform.position, transform.forward * range, 3, false, Color.red);
        }
    }
}