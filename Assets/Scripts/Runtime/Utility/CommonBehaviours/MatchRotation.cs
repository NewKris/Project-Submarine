using System;
using UnityEngine;

namespace WereHorse.Runtime.Utility.CommonBehaviours {
    public class MatchRotation : MonoBehaviour {
        public Transform target;

        private void Update() {
            transform.rotation = target.rotation;
        }
    }
}