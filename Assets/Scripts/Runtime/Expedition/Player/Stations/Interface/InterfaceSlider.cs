using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceSlider : FloatControl {
        private Vector3 _offsetDrag;
        private Plane _handlePlane;

        public override void OnHandleStart() {
            isDragging = true;
            _offsetDrag = ProjectMouse() - handle.localPosition;
        }
        
        public override void OnHandleStop() {
            isDragging = false;
        }

        protected override void Start() {
            base.Start();
            _handlePlane = new Plane(transform.up, transform.position);
        }

        protected override void SetHandleTransform(float newValue) {
            handle.localPosition = Vector3.forward * CalculateTransformAmount(newValue);
        }

        protected override float IntegrateTransform() {
            Vector3 currentDrag = ProjectMouse();
            Vector3 pos = currentDrag - _offsetDrag;
            return Mathf.Clamp(pos.z, minTransform, maxTransform);
        }

        private Vector3 ProjectMouse() {
            Ray ray = Camera.main.ScreenPointToRay(StationInputListener.MousePosition);
            _handlePlane.Raycast(ray, out float enter);
            return transform.InverseTransformPoint(ray.GetPoint(enter));
        }
    }
}