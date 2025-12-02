using System;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class Interior : NetworkBehaviourExtended {
        public Transform exteriorBody;

        private void Update() {
            DoOnServer(() => {
                float roll = CalculatePitch(); 
                float pitch = CalculatePitch();
                
                transform.rotation = Quaternion.Euler(pitch, 0, roll);
            });
        }

        private float CalculateRoll() {
            return Vector3.Angle(Vector3.up, exteriorBody.up);
        }

        private float CalculatePitch() {
            Vector3 flatForward = exteriorBody.forward;
            flatForward.y = 0;
            flatForward.Normalize();

            return Vector3.Angle(flatForward, exteriorBody.forward);
        }
    }
}