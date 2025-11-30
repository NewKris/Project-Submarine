using UnityEngine;

namespace WereHorse.Runtime.Expedition.Interaction {
    public class TestInteractable : Interactable {
        public override void Interact() {
            Debug.Log("!");
        }
    }
}