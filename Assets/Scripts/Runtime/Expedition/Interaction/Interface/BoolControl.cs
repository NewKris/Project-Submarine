using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public abstract class BoolControl : InterfaceControl {
        public bool defaultValue;
        public UnityEvent<bool> onValueChanged;

        [Header("Transform")] 
        public Transform handle;
        public float onTransform;
        public float offTransform;
        public float pressedTransform;

        private readonly NetworkVariable<bool> _value = new();

        public override void OnHandleStart() {
            SetHandleTransform(pressedTransform);
        }

        public override void OnHandleStop() {
            _value.Value = !_value.Value;
            SetHandleTransform(_value.Value ? onTransform : offTransform);
        }

        protected abstract void SetHandleTransform(float amount);

        private void OnValidate() {
            if (handle) {
                SetHandleTransform(defaultValue ? onTransform : offTransform);
            }
        }
    }
}