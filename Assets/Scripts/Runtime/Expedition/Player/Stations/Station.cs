using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class Station : MonoBehaviour {
        public Transform stationPivot;
        public float cameraDirection;
        [ReadOnly] public bool occupied;

        private void OnDrawGizmos() {
            if (stationPivot) {
                Vector3 cameraDir = Quaternion.AngleAxis(cameraDirection, Vector3.right) * Vector3.forward;
                cameraDir = stationPivot.TransformDirection(cameraDir).normalized;
                
                HandlesProxy.DrawDisc(stationPivot.position, Vector3.up, 0.5f, true, Color.yellow);
                HandlesProxy.DrawRay(stationPivot.position + Vector3.up * 1.6f, cameraDir, 3, false, Color.red);
            }
        }
    }
}