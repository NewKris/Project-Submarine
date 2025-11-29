using UnityEngine;

namespace WereHorse.Runtime.Gameplay.Interaction {
    public class TestInteraction : Interaction {
        public override void Interact() {
            Debug.Log("!");
        }
    }
}