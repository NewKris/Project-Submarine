using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceSlider : InterfaceControl {
        public Transform handle;
        public float maxHandlePosition;
        public float minHandlePosition;
        [Range(0, 1)] public float defaultValue;
        public UnityEvent<float> onValueChanged;

        private bool _isDragging;
        private Vector3 _offsetDrag;
        private Vector3 _currentDrag;
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
            DoOnServer(() => {
            });
            
            DoOnAll(() => {
                _handlePlane = new Plane(transform.up, transform.position);
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
                _currentDrag = ProjectMouse();
                Vector3 pos = _currentDrag - _offsetDrag;
                float handlePos = Mathf.Clamp(pos.z, minHandlePosition, maxHandlePosition);
                SetValueRpc(CalculateValue(handlePos));
            }
        }

        private float CalculateValue(float handlePosition) {
            return Mathf.InverseLerp(minHandlePosition, maxHandlePosition, handlePosition);
        }

        private float CalculateHandlePosition(float value) {
            return Mathf.Lerp(minHandlePosition, maxHandlePosition, value);
        }
        
        [Rpc(SendTo.Server)]
        private void SetValueRpc(float newValue) {
            handle.localPosition = Vector3.forward * CalculateHandlePosition(newValue);
            _value.Value = newValue;
            onValueChanged.Invoke(newValue);
        }

        private Vector3 ProjectMouse() {
            Ray ray = Camera.main.ScreenPointToRay(StationInputListener.MousePosition);
            _handlePlane.Raycast(ray, out float enter);
            return transform.TransformPoint(ray.GetPoint(enter));
        }
    }
}