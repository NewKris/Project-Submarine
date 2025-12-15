using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;

namespace WereHorse.Runtime.Expedition.Stations {
    public class StationInteractable : Interactable {
        public static event Action<Station>  OnInteracted; 
        
        public Station station;
        
        public override void Interact() {
            if (!station) {
                Debug.LogError("[!] Cannot possess a null station!");
                return;
            }
            
            OnInteracted?.Invoke(station);
        }
    }
}