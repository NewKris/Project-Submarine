using UnityEngine;

namespace WereHorse.Runtime.Gameplay.Interaction {
    public class TestInteractable : Interactable {
        public override void Interact() {
            Debug.Log("!");
        }
    }
}