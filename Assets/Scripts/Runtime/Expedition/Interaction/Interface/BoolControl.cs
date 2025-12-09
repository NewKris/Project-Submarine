using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public abstract class BoolControl : InterfaceControl {
        public bool defaultValue;
        public bool binaryState = true;
        public bool canInteractDefault;
        public UnityEvent<bool> onValueChanged;

        [Header("Transform")] 
        public Transform handle;
        public float onTransform;
        public float offTransform;
        
        [Header("Indicators")] 
        public BoolIndicator[] indicators;
        public BoolIndicator interactableIndicator;

        private readonly NetworkVariable<bool> _value = new();
        private readonly NetworkVariable<bool> _canInteract = new();

        public override void OnHandleStart() {
            SetHandleTransform(onTransform);
        }

        public override void OnHandleStop() {
            if (_canInteract.Value) {
                SetValueRpc(!_value.Value);
            }
            else {
                SetHandleTransform(CalculateTransformAmount(_value.Value));
            }
        }

        public void SetInteractable(bool value) {
            DoOnServer(() => {
                _canInteract.Value = value;
            });
        }

        protected abstract void SetHandleTransform(float amount);

        private void Start() {
            DoOnServer(() => {
                SetInteractable(canInteractDefault);
                SetValueRpc(defaultValue);
            });
            
            DoOnAll(() => {
                _value.OnValueChanged += (_, newVal) => {
                    SetHandleTransform(CalculateTransformAmount(newVal));
                    onValueChanged.Invoke(newVal);
                };
                
                HookIndicatorListeners();
                SetHandleTransform(CalculateTransformAmount(_value.Value));

                if (binaryState) {
                    onValueChanged.Invoke(_value.Value);
                }
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
            return value && binaryState ? onTransform : offTransform;
        }
        
        private void HookIndicatorListeners() {
            foreach (BoolIndicator boolIndicator in indicators) {
                onValueChanged.AddListener(boolIndicator.UpdateValue);
            }

            if (interactableIndicator) {
                _canInteract.OnValueChanged += (_, newVal) => {
                    interactableIndicator.UpdateValue(newVal);
                };
            }
        }
    }
}