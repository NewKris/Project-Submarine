using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Tasks {
    public class AreaOverlap : MonoBehaviour {
        public LayerMask layerMask;

        public Collider[] Evaluate() {
            return Physics.OverlapBox(transform.position, transform.localScale * 0.5f, transform.rotation, layerMask);
        }

        private void OnDrawGizmos() {
            HandlesProxy.DrawCube(transform.position, transform.localScale, transform.rotation, Color.green, true, 1f);
        }
    }
}