using System;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Gameplay.Interaction {
    public class InteractionController : MonoBehaviour {
        public float range;
        public LayerMask interactionMask;
        public GameObject prompt;

        private Interaction _interaction;
        
        public void TryInteract() {
            if (_interaction) {
                _interaction.Interact();
            }
        }

        private void Update() {
            _interaction = FindInteraction();
        }

        private Interaction FindInteraction() {
            Ray ray = new Ray(transform.position, transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, interactionMask)) {
                prompt.SetActive(true);
                return hit.collider.GetComponent<Interaction>();
            }

            prompt.SetActive(false);
            return null;
        }
        
        private void OnDrawGizmos() {
            HandlesProxy.DrawRay(transform.position, transform.forward * range, 3, false, Color.red);
        }
    }
}