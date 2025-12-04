using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceSlider : InterfaceControl {
        public Transform handle;
        public float maxHandlePosition;
        public float minHandlePosition;
        [Range(0, 1)] public float defaultValue;
        public UnityEvent<float> onValueChanged;

        [Header("Snapping")] 
        public bool snapToValues;
        public float[] snapValues;
        public float snapRange;
        public bool allowLiminalValues;

        private bool _isDragging;
        private Vector3 _offsetDrag;
        private Plane _handlePlane;
        private readonly NetworkVariable<float> _value = new ();

        public override void OnHandleStart() {
            _isDragging = true;
            _offsetDrag = ProjectMouse() - handle.localPosition;
        }
        
        public override void OnHandleStop() {
            _isDragging = false;
        }

        private void Start() {
            DoOnAll(() => {
                _handlePlane = new Plane(transform.up, transform.position);
                handle.localPosition = Vector3.forward * CalculateHandlePosition(_value.Value);
            });
        }

        private void OnValidate() {
            if (handle) {
                handle.localPosition = Vector3.Lerp(
                    Vector3.forward * minHandlePosition, 
                    Vector3.forward * maxHandlePosition, 
                    defaultValue
                );
            }
        }

        private void Update() {
            if (_isDragging) {
                Vector3 currentDrag = ProjectMouse();
                Vector3 pos = currentDrag - _offsetDrag;
                float handlePos = Mathf.Clamp(pos.z, minHandlePosition, maxHandlePosition);
                SetValueRpc(CalculateValue(handlePos));
            }
        }
        
        [Rpc(SendTo.Server)]
        private void SetValueRpc(float newValue) {
            _value.Value = SnapValue(newValue);
            handle.localPosition = Vector3.forward * CalculateHandlePosition(_value.Value);
            onValueChanged.Invoke(_value.Value);
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
        
        private float CalculateValue(float handlePosition) {
            return Mathf.InverseLerp(minHandlePosition, maxHandlePosition, handlePosition);
        }

        private float CalculateHandlePosition(float value) {
            return Mathf.Lerp(minHandlePosition, maxHandlePosition, value);
        }

        private Vector3 ProjectMouse() {
            Ray ray = Camera.main.ScreenPointToRay(StationInputListener.MousePosition);
            _handlePlane.Raycast(ray, out float enter);
            return transform.InverseTransformPoint(ray.GetPoint(enter));
        }
    }
}