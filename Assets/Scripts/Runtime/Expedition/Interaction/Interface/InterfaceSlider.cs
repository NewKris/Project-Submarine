using UnityEngine;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
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

        protected override void SetHandleTransform(float offset) {
            handle.localPosition = Vector3.forward * offset;
        }

        protected override float IntegrateTransform() {
            Vector3 currentDrag = ProjectMouse();
            Vector3 pos = currentDrag - _offsetDrag;
            return Mathf.Clamp(pos.z, minTransform, maxTransform);
        }

        private Vector3 ProjectMouse() {
            Ray ray = Camera.main.ScreenPointToRay(PlayerInputListener.MousePosition);
            _handlePlane.Raycast(ray, out float enter);
            return transform.InverseTransformPoint(ray.GetPoint(enter));
        }
    }
}