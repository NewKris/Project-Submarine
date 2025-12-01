using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class Telescope : NetworkBehaviourExtended {
        [Header("Yaw")]
        public float maxYawSpeed;
        public Transform yawPivot;
        
        [Header("Pitch")]
        public float maxPitchSpeed;
        public Transform pitchPivot;
        
        [Rpc(SendTo.Server)]
        public void LookRpc(float yaw, float pitch) {
            
        }
    }
}