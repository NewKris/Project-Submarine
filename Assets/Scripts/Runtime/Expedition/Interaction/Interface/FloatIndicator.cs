using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public class FloatIndicator : MonoBehaviour {
        public float activateAtValue;
        public UnityEvent<bool> onValueChanged;
        
        public void UpdateValue(float newValue) {
            bool isActive = Mathf.Abs(activateAtValue - newValue) <= 0.001f;
            onValueChanged.Invoke(isActive);
        }
    }
}