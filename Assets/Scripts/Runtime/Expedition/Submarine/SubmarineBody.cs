using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class SubmarineBody : NetworkBehaviourExtended {
        public Rigidbody rigidBody;
        
        [Header("Thrust")]
        public float thrustAcceleration;
        public float deadZone;
        
        [Header("Lift")]
        public float liftAcceleration;
        
        [Header("Yaw")]
        public float rotationAcceleration;
        
        [Header("Thrusters")]
        public Transform leftThruster;
        public Transform rightThruster;

        private float _thrust;
        private float _yaw;
        private float _lift;

        public float Thrust {
            get => _thrust;
            set {
                if (IsServer) {
                    _thrust = value;
                }
            }
        }
        
        public float Yaw {
            get => _yaw;
            set {
                if (IsServer) {
                    _yaw = value;
                }
            }
        }
        
        public float Lift {
            get => _lift;
            set {
                if (IsServer) {
                    _lift = value;
                }
            }
        }
        
        private void FixedUpdate() {
            DoOnServer(() => {
                rigidBody.AddForce(transform.forward * CalculateThrustForce(), ForceMode.Acceleration);
                rigidBody.AddForce(transform.up * (Lift * liftAcceleration), ForceMode.Acceleration);
                
                Transform activeThruster = Yaw < 0 ? rightThruster : leftThruster;
                rigidBody.AddForceAtPosition(
                    activeThruster.forward * Mathf.Abs(Yaw * rotationAcceleration), 
                    activeThruster.position, 
                    ForceMode.Acceleration
                );
            });
        }

        private float CalculateThrustForce() {
            float thrust = Mathf.Lerp(-1, 3f, Thrust);
            if (Mathf.Abs(thrust) < deadZone) {
                thrust = 0;
            }

            return thrust * thrustAcceleration;
        }
    }
}