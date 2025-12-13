using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Vehicle.Tank {
    public class TankVehicle : MonoBehaviour {
        public Rigidbody rigidBody;
        public Transform[] springs;

        [Header("Drive")]
        public float maxSpringPitch;
        public float maxSpringRoll;
        public float maxRotateSpeed;
        public List<int> leftSprings;
        public List<int> rightSprings;
        
        [Header("Friction")]
        [Range(0, 1)] public float friction;
        
        [Header("Suspension")]
        public float length;
        public float restLength;
        public float stiffness;
        public float damping;
        public LayerMask groundMask;

        private bool[] _springGrounded;
        private Vector3[] _springVelocities;
        private RaycastHit[] _springHits;
        private Quaternion[] _springRots;

        private void Start() {
            _springGrounded = new bool[springs.Length];
            _springVelocities = new Vector3[springs.Length];
            _springHits = new RaycastHit[springs.Length];
            _springRots = new Quaternion[springs.Length];
        }

        private void FixedUpdate() {
            CacheSpringVelocities();
            CacheSpringRayCasts();
            
            ApplySuspensionForces();
            ApplyFrictionForces();
            
            CalculateSpringRotations();
            LerpSpringRotations();
        }

        private void CalculateSpringRotations() {
            float targetRoll = -maxSpringRoll * VehicleInputListener.Move.x;
            float targetLeftPitch = CalculateLeftPitch();
            float targetRightPitch = CalculateRightPitch();
            
            ForEachLeftSpring(i => {
                _springRots[i] = Quaternion.Euler(targetLeftPitch, 0, targetRoll);
            });
            
            ForEachRightSpring(i => {
                _springRots[i] = Quaternion.Euler(targetRightPitch, 0, targetRoll);
            });
        }

        private float CalculateRightPitch() {
            float rightPitchOffset = -VehicleInputListener.Rotate;
            float rightPitchAmount = Mathf.Clamp(VehicleInputListener.Move.y + rightPitchOffset, -1, 1);
            return maxSpringPitch * rightPitchAmount;
        }

        private float CalculateLeftPitch() {
            float leftPitchOffset = VehicleInputListener.Rotate;
            float leftPitchAmount = Mathf.Clamp(VehicleInputListener.Move.y + leftPitchOffset, -1, 1);
            return maxSpringPitch * leftPitchAmount;
        }

        private void LerpSpringRotations() {
            float maxDelta = maxRotateSpeed * Time.fixedDeltaTime;
            
            ForEachSpring(i => {
                springs[i].localRotation = Quaternion.RotateTowards(springs[i].localRotation, _springRots[i], maxDelta);
            });
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
                    ForceMode.Acceleration
                );
                
                _springVelocities[i] += force * Time.fixedDeltaTime;
            });
        }

        private void ApplySuspensionForces() {
            ForEachSpring(i => {
                Vector3 force = GetSuspensionForces(i);
                
                rigidBody.AddForceAtPosition(
                    force,
                    springs[i].position,
                    ForceMode.Acceleration
                );

                _springVelocities[i] += force * Time.fixedDeltaTime;
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
            foreach (Transform spring in springs) {
                HandlesProxy.DrawRay(spring.position, -spring.up * length, 3, false, Color.red);
                HandlesProxy.DrawSphere(spring.position - spring.up * restLength, 0.1f, false, Color.green);
            }
        }

        private void ForEachLeftSpring(Action<int> callback) {
            foreach (int leftSpring in leftSprings) {
                callback(leftSpring);
            }
        }
        
        private void ForEachRightSpring(Action<int> callback) {
            foreach (int rightSpring in rightSprings) {
                callback(rightSpring);
            }
        }

        private void ForEachSpring(Action<int> callback) {
            for (int i = 0; i < springs.Length; i++) {
                callback(i);
            }
        }
    }
}