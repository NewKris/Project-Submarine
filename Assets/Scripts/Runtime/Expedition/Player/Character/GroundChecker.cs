using System;
using UnityEngine;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class GroundChecker : MonoBehaviour {
        public float radius;
        public float length;
        public LayerMask layerMask;

        public bool Evaluate() {
            return Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit _, length, layerMask);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.DrawWireSphere(transform.position + Vector3.down * length, radius);
        }
    }
}