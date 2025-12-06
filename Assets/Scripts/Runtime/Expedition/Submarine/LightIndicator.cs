using System;
using UnityEngine;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class LightIndicator : MonoBehaviour {
        public Material onMaterial;
        public Material offMaterial;
        public MeshRenderer meshRenderer;
        
        public void ToggleLight(bool isActive) {
            meshRenderer.material = isActive ? onMaterial : offMaterial;
        }
    }
}