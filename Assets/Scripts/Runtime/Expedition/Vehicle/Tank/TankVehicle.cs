using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Vehicle.Tank {
    public class TankVehicle : MonoBehaviour {
        public Rigidbody rigidBody;
        public Transform[] springs;

        [Header("Drive")] 
        public float acceleration;
        public float maxDriveSpeed;
        public float maxStrafeSpeed;

        [Header("Rotate")] 
        public float torqueAcceleration;
        public float maxRotateSpeed;
        
        [Header("Friction")]
        [Range(0, 1)] public float friction;
        
        [Header("Suspension")]
        public float length;
        public float restLength;
        public float stiffness;
        public float damping;
        public LayerMask groundMask;

        [Header("Test")] 
        [Range(0, 1)] public float drive;
        [Range(0, 1)] public float strafe;
        [Range(0, 1)] public float turn;

        private bool[] _springGrounded;
        private Vector3[] _springVelocities;
        private RaycastHit[] _springHits;
        
        private float Drive => Mathf.Lerp(-1, 1, drive);
        private float Strafe => Mathf.Lerp(-1, 1, strafe);
        private float Turn => Mathf.Lerp(-1, 1, turn);

        private void Start() {
            _springGrounded = new bool[springs.Length];
            _springVelocities = new Vector3[springs.Length];
            _springHits = new RaycastHit[springs.Length];
        }

        private void FixedUpdate() {
            CacheSpringVelocities();
            CacheSpringRayCasts();
            
            ApplySuspensionForces();
            ApplyFrictionForces();
            
            ApplyTurning();
            ApplyDriveVelocity();
        }

        private void ApplyDriveVelocity() {
            Vector3 targetVelocity = new Vector3(maxStrafeSpeed * Strafe, 0, maxDriveSpeed * Drive);
            targetVelocity = transform.rotation * targetVelocity;
            
            Vector3 delta =  targetVelocity - rigidBody.linearVelocity;
            rigidBody.AddForce(delta * acceleration, ForceMode.Acceleration);
        }

        private void ApplyTurning() {
            Vector3 targetTorque = new Vector3(0, maxRotateSpeed * Turn, 0);
            targetTorque = transform.rotation * targetTorque;

            Vector3 delta = targetTorque - rigidBody.angularVelocity;
            rigidBody.AddTorque(delta * torqueAcceleration, ForceMode.Acceleration);
        }
        
        private void CacheSpringVelocities() {
            ForEachSpring(i => {
                _springVelocities[i] =  rigidBody.GetPointVelocity(springs[i].position);
            });
        }

        private void CacheSpringRayCasts() {
            ForEachSpring(i => {
                _springGrounded[i] = CastSpring(springs[i], out _springHits[i]);
            });
        }
        
        private void ApplyFrictionForces() {
            ForEachSpring(i => {
                Vector3 force = GetFrictionForces(i); 
                
                rigidBody.AddForceAtPosition(
                    force,
                    springs[i].position,
                    ForceMode.Force
                );
                
                _springVelocities[i] += force * Time.fixedDeltaTime / rigidBody.mass;
            });
        }

        private void ApplySuspensionForces() {
            ForEachSpring(i => {
                Vector3 force = GetSuspensionForces(i);
                
                rigidBody.AddForceAtPosition(
                    force,
                    springs[i].position,
                    ForceMode.Force
                );

                _springVelocities[i] += force * Time.fixedDeltaTime / rigidBody.mass;
            });
        }

        private Vector3 GetFrictionForces(int i) {
            Vector3 frictionForce = Vector3.zero;
            
            if (_springGrounded[i]) {
                Vector3 slideVel = Vector3.ProjectOnPlane(_springVelocities[i], _springHits[i].normal);
                frictionForce = -slideVel * friction;
            }

            return frictionForce;
        }
        
        private Vector3 GetSuspensionForces(int i) {
            Vector3 springDir = springs[i].up;
            
            float force = 0;
            
            if (_springGrounded[i]) {
                float offset = restLength - _springHits[i].distance;
                float vel = Vector3.Dot(springDir, _springVelocities[i]);
                
                force = (offset * stiffness) - (vel * damping);
            }
            
            return springDir * force;
        }

        private bool CastSpring(Transform spring, out RaycastHit hit) {
            return Physics.Raycast(spring.position, -spring.up, out hit, length, groundMask);
        }
        
        private void OnDrawGizmos() {
            if (springs == null) {
                return;
            }
            
            foreach (Transform spring in springs) {
                HandlesProxy.DrawRay(spring.position, -spring.up * length, 3, false, Color.red);
                HandlesProxy.DrawSphere(spring.position - spring.up * restLength, 0.1f, false, Color.green);
            }
        }

        private void ForEachSpring(Action<int> callback) {
            for (int i = 0; i < springs.Length; i++) {
                callback(i);
            }
        }
    }
}