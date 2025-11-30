using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Player {
    public class Portal : Interactable {
        public Transform toPoint;
        
        public override void Interact() {
            PlayerCharacter.ownedCharacter.SetPositionAndRotationRpc(toPoint.position, toPoint.rotation);
        }

        private void OnDrawGizmos() {
            if (toPoint) {
                HandlesProxy.DrawLine(transform.position, toPoint.position, 1, true, Color.red);
            }
        }
    }
}