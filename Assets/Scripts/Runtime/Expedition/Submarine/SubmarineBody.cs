using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class SubmarineBody : NetworkBehaviourExtended {
        public Rigidbody rigidBody;
        public float thrustAcceleration;
        public float liftAcceleration;
        public float rotationAcceleration;
        
        [Header("Thrusters")]
        public Transform leftThruster;
        public Transform rightThruster;
        
        public float Thrust { get; set; }
        public float Yaw { get; set; }
        public float Lift { get; set; }
        
        private void FixedUpdate() {
            DoOnServer(() => {
                rigidBody.AddForce(transform.forward * (Thrust * thrustAcceleration), ForceMode.Acceleration);
                rigidBody.AddForce(transform.up * (Lift * liftAcceleration), ForceMode.Acceleration);
                
                Transform activeThruster = Yaw < 0 ? rightThruster : leftThruster;
                rigidBody.AddForceAtPosition(
                    activeThruster.forward * Mathf.Abs(Yaw * rotationAcceleration), 
                    activeThruster.position, 
                    ForceMode.Acceleration
                );
            });
        }
    }
}