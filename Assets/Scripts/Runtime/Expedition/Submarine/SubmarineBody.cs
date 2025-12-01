using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class SubmarineBody : NetworkBehaviourExtended {
        public Rigidbody rigidBody;
        
        [Header("Moving")]
        public float maxMoveSpeed;
        public float maxAcceleration;
        
        [Header("Rotating")]
        public float maxAngularSpeed;
        public float maxAngularAcceleration;

        private float _thrust;
        private float _yaw;
        private float _lift;

        public bool Accelerating => Mathf.Abs(_thrust) + Mathf.Abs(_yaw) + Mathf.Abs(_lift) > 0;
        
        [Rpc(SendTo.Server)]
        public void SendSteerValuesRpc(float thrust, float yaw, float lift) {
            _thrust = thrust;
            _yaw = yaw;
            _lift = lift;
        }

        private void FixedUpdate() {
            DoOnServer(() => {
                Vector3 targetVel = new Vector3(0, _lift, _thrust).normalized * maxMoveSpeed;
                targetVel = rigidBody.rotation * targetVel;
                Vector3 nextVel = Vector3.MoveTowards(rigidBody.linearVelocity, targetVel, maxAcceleration * Time.fixedDeltaTime);
                Vector3 delta = nextVel - rigidBody.linearVelocity;
                rigidBody.AddForce(delta, ForceMode.VelocityChange);

                Vector3 targetTorque = new Vector3(0, _yaw * maxAngularSpeed, 0);
                Vector3 nextTorque = Vector3.MoveTowards(rigidBody.angularVelocity, targetTorque, maxAngularAcceleration * Time.fixedDeltaTime);
                Vector3 deltaTorque = nextTorque - rigidBody.angularVelocity;
                rigidBody.AddTorque(deltaTorque, ForceMode.VelocityChange);
            });
        }
    }
}