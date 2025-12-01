using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Player {
    public class Portal : Interactable {
        public Transform toPoint;
        
        public override void Interact() {
            PlayerCharacter.ownedCharacter.SetPositionAndRotation(toPoint.position, toPoint.rotation);
        }

        private void OnDrawGizmos() {
            if (toPoint) {
                HandlesProxy.DrawLine(transform.position, toPoint.position, 1, true, Color.red);
                HandlesProxy.DrawDisc(toPoint.position, Vector3.up, 0.5f, true, Color.red, 3);
            }
        }
    }
}