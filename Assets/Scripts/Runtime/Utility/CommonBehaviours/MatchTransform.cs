using System;
using UnityEngine;

namespace WereHorse.Runtime.Utility.CommonBehaviours {
    public class MatchTransform : MonoBehaviour {
        public Transform target;

        private void OnValidate() {
            if (target) {
                transform.position = target.position;
                transform.rotation = target.rotation;
            }
        }

        private void Update() {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}