using System;
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

        private readonly NetworkVariable<bool> _value = new();

        public override void OnHandleStart() { }

        public override void OnHandleStop() {
            SetValueRpc(!_value.Value);
        }

        protected abstract void SetHandleTransform(float amount);

        private void Start() {
            DoOnServer(() => {
                SetValueRpc(defaultValue);
            });
            
            DoOnAll(() => {
                _value.OnValueChanged += (_, newVal) => {
                    SetHandleTransform(CalculateTransformAmount(newVal));
                    onValueChanged.Invoke(newVal);
                };
                
                SetHandleTransform(CalculateTransformAmount(_value.Value));
                onValueChanged.Invoke(_value.Value);
            });
        }

        private void OnValidate() {
            if (handle) {
                SetHandleTransform(CalculateTransformAmount(defaultValue));
            }
        }
        
        [Rpc(SendTo.Server)]
        private void SetValueRpc(bool newValue) {
            _value.Value = newValue;
        }
        
        private float CalculateTransformAmount(bool value) {
            return value ? onTransform : offTransform;
        }
    }
}