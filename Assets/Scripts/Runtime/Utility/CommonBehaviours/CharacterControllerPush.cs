using UnityEngine;

namespace Werehorse.Runtime.Utility.CommonBehaviours {
    public class CharacterControllerPush : MonoBehaviour {
        public float pushForce;
        
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic) {
                return;
            }

            body.AddForce(hit.moveDirection * pushForce, ForceMode.Impulse);
        }
    }
}
