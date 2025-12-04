using UnityEngine;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class LeverControl : FloatControl {
        [Header("Lever Settings")]
        public float sensitivity;
        
        public override void OnHandleStart() {
            isDragging = true;
        }
        
        public override void OnHandleStop() {
            isDragging = false;
        }
        
        protected override void SetHandleTransform(float newValue) {
            float rot = CalculateTransformAmount(newValue);
            handle.localRotation = Quaternion.Euler(rot, 0, 0);
        }
        
        protected override float IntegrateTransform() {
            float rot = StationInputListener.DeltaMouse.y * sensitivity;
            float rotation = handle.localRotation.eulerAngles.x - rot;
            return Mathf.Clamp(rotation, minTransform, maxTransform);
        }
    }
}