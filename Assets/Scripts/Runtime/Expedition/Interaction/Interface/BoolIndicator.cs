using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public class BoolIndicator : MonoBehaviour {
        public bool activeAtValue;
        public UnityEvent<bool> onValueChanged;

        public void UpdateValue(bool newValue) {
            bool isActive = activeAtValue == newValue;
            onValueChanged.Invoke(isActive);
        }
    }
}