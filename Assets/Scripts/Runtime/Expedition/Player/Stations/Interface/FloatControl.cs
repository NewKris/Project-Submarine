using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public abstract class FloatControl : InterfaceControl {
        [Range(0, 1)] public float defaultValue;
        public UnityEvent<float> onValueChanged;
        
        [Header("Transform")]
        public Transform handle;
        public float minTransform;
        public float maxTransform;
        
        [Header("Snapping")] 
        public bool snapToValues;
        public float[] snapValues;
        public float snapRange;
        public bool allowLiminalValues;

        protected bool isDragging;
        
        private readonly NetworkVariable<float> _value = new ();

        protected abstract void SetHandleTransform(float newValue);
        protected abstract float IntegrateTransform();
        
        protected virtual void Start() {
            DoOnServer(() => {
                SetValueRpc(defaultValue);
            });
            
            DoOnAll(() => {
                SetHandleTransform(defaultValue);
                _value.OnValueChanged += (_, newVal) => SetHandleTransform(newVal);
                enabled = false;
            });
        }
        
        protected float CalculateTransformAmount(float value) {
            return Mathf.Lerp(minTransform, maxTransform, value);
        }
        
        private void OnValidate() {
            if (handle) {
                SetHandleTransform(defaultValue);
            }
        }
        
        private void Update() {
            if (isDragging) {
                float newValue = CalculateValue(IntegrateTransform());
                newValue = SnapValue(newValue);
                
                SetValueRpc(newValue);
            }
        }
        
        private float CalculateValue(float transformAmount) {
            return Mathf.InverseLerp(minTransform, maxTransform, transformAmount);
        }

        [Rpc(SendTo.Server)]
        private void SetValueRpc(float newValue) {
            _value.Value = newValue;
            onValueChanged.Invoke(newValue);
        }
        
        private float SnapValue(float realValue) {
            if (!snapToValues) {
                return realValue;
            }
            
            foreach (float snapValue in snapValues) {
                if (Mathf.Abs(realValue - snapValue) < snapRange) {
                    return snapValue;
                }
            }

            if (!allowLiminalValues) {
                return _value.Value;
            }

            return realValue;
        }
    }
}