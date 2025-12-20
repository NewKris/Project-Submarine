using System;
using UnityEngine;
using WereHorse.Runtime.Utility;
using Random = UnityEngine.Random;

namespace WereHorse.Runtime.Expedition.Tasks {
    public class TaskSpawn : MonoBehaviour {
        public float radius;
        public float offset;

        public Vector3 GetRandomPoint() {
            Vector2 rand = Random.insideUnitCircle * radius;
            Vector3 point = transform.TransformPoint(new Vector3(rand.x, 0, rand.y));
            
            Ray ray = new Ray(point, Vector3.down);
            Physics.Raycast(ray, out RaycastHit hit);
            
            return hit.point + Vector3.up * offset;
        }
        
        private void OnDrawGizmos() {
            HandlesProxy.DrawDisc(transform.position, Vector3.up, radius, true, Color.yellow, 1f);
        }
    }
}