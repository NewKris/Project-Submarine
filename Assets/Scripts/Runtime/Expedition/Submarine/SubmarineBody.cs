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
        
        private float _thrust;
        private float _yaw;
        private float _lift;
        private readonly NetworkVariable<bool> _accelerating = new ();

        public bool Accelerating => _accelerating.Value;
        
        [Rpc(SendTo.Server)]
        public void SendSteerValuesRpc(float thrust, float yaw, float lift) {
            _thrust = thrust;
            _yaw = yaw;
            _lift = lift;
            
            _accelerating.Value = Mathf.Abs(_thrust) + Mathf.Abs(_yaw) + Mathf.Abs(_lift) > 0;
        }

        private void FixedUpdate() {
            DoOnServer(() => {
                rigidBody.AddForce(transform.forward * (_thrust * thrustAcceleration), ForceMode.Acceleration);
                rigidBody.AddForce(transform.up * (_lift * liftAcceleration), ForceMode.Acceleration);
                
                Transform activeThruster = _yaw < 0 ? rightThruster : leftThruster;
                rigidBody.AddForceAtPosition(
                    activeThruster.forward * Mathf.Abs(_yaw * rotationAcceleration), 
                    activeThruster.position, 
                    ForceMode.Acceleration
                );
            });
        }
    }
}