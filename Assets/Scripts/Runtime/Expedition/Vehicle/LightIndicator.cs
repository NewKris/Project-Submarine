using UnityEngine;

namespace WereHorse.Runtime.Expedition.Vehicle {
    public class LightIndicator : MonoBehaviour {
        public Material onMaterial;
        public Material offMaterial;
        public MeshRenderer meshRenderer;
        
        public void ToggleLight(bool isActive) {
            meshRenderer.material = isActive ? onMaterial : offMaterial;
        }
    }
}