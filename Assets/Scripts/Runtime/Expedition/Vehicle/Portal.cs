using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Vehicle {
    public class Portal : Interactable {
        public static event Action<Vector3, Quaternion> OnInteracted;
        
        public Transform toPoint;
        
        public override void Interact() {
            OnInteracted?.Invoke(toPoint.position, toPoint.rotation);
        }

        private void OnDrawGizmos() {
            if (toPoint) {
                HandlesProxy.DrawLine(transform.position, toPoint.position, 1, true, Color.red);
                HandlesProxy.DrawDisc(toPoint.position, Vector3.up, 0.5f, true, Color.red, 3);
            }
        }

        private void Reset() {
            gameObject.layer = LayerMask.NameToLayer("Interaction");
        }
    }
}