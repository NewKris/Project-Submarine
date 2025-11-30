using System;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public abstract class Station : MonoBehaviour {
        public Transform stationPivot;

        public abstract void Activate();

        public abstract void Deactivate();

        private void OnDrawGizmos() {
            HandlesProxy.DrawDisc(stationPivot.position, Vector3.up, 0.5f, true, Color.yellow);
        }
    }
}