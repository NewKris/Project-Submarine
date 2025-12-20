using System;
using UnityEngine;
using WereHorse.Runtime.Utility;
using Random = UnityEngine.Random;

namespace WereHorse.Runtime.Expedition.Tasks {
    public class TaskSpawn : MonoBehaviour {
        public float radius;
        public float offset;

        public Vector3 GetRandomPoint() {
            Vector3 rand = Random.insideUnitCircle * radius;
            Ray ray = new Ray(transform.TransformPoint(rand), Vector3.down);
            
            Physics.Raycast(ray, out RaycastHit hit);
            return hit.point + Vector3.up * offset;
        }
        
        private void OnDrawGizmos() {
            HandlesProxy.DrawDisc(transform.position, Vector3.up, radius, true, Color.yellow, 1f);
        }
    }
}