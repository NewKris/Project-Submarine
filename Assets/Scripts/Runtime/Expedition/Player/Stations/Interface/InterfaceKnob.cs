using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceKnob : FloatControl {
        [Header("Knob Settings")]
        public float sensitivity;
        
        public override void OnHandleStart() {
            isDragging = true;
        }
        public override void OnHandleStop() {
            isDragging = false;
        }

        protected override void SetHandleTransform(float newValue) {
            float rot = CalculateTransformAmount(newValue);
            handle.localRotation = Quaternion.Euler(0, rot, 0);
        }

        protected override float IntegrateTransform() {
            float rot = StationInputListener.DeltaMouse.x * sensitivity;
            float rotation = handle.localRotation.eulerAngles.y + rot;
            return Mathf.Clamp(rotation, minTransform, maxTransform);
        }
    }
}