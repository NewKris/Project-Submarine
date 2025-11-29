using System;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Gameplay.Player {
    public class Portal : Interaction.Interaction {
        public Transform toPoint;
        
        public override void Interact() {
            PlayerCharacter.OwnedCharacter.SetPositionAndRotationRpc(toPoint.position, toPoint.rotation);
        }

        private void OnDrawGizmos() {
            if (toPoint) {
                HandlesProxy.DrawLine(transform.position, toPoint.position, 1, true, Color.red);
            }
        }
    }
}