using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class SubmarineBody : NetworkBehaviourExtended {
        public Rigidbody rigidBody;
        
        [Header("Acceleration")]
        public float deadZone;
        public float thrustAcceleration;
        public float liftAcceleration;
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
                rigidBody.AddForce(transform.up * CalculateLiftForce(), ForceMode.Acceleration);
                
                Transform activeThruster = Yaw < 0.5f ? rightThruster : leftThruster;
                rigidBody.AddForceAtPosition(
                    activeThruster.forward * CalculateYawForce(), 
                    activeThruster.position, 
                    ForceMode.Acceleration
                );
            });
        }

        private float CalculateYawForce() {
            float yaw = Mathf.Abs(Yaw - 0.5f) * 2;

            if (yaw < deadZone) {
                yaw = 0;
            }

            return yaw * rotationAcceleration;
        }

        private float CalculateLiftForce() {
            float lift = Mathf.Lerp(-1, 1, Lift);
            return lift * liftAcceleration;
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