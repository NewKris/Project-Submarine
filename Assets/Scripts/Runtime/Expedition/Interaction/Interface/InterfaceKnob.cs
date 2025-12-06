using UnityEngine;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public class InterfaceKnob : FloatControl {
        [Header("Knob Settings")]
        public float sensitivity;

        private float _targetRot;
        
        public override void OnHandleStart() {
            isDragging = true;
            _targetRot = handle.localRotation.eulerAngles.y;
        }
        public override void OnHandleStop() {
            isDragging = false;
        }

        protected override void SetHandleTransform(float rot) {
            handle.localRotation = Quaternion.Euler(0, rot, 0);
        }

        protected override float IntegrateTransform() {
            float rot = PlayerInputListener.Look.x * sensitivity;
            _targetRot += rot;
            _targetRot = Mathf.Clamp(_targetRot, minTransform, maxTransform);
            
            return _targetRot;
        }
    }
}