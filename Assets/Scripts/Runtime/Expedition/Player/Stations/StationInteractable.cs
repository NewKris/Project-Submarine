using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class StationInteractable : Interactable {
        public Station station;
        
        public override void Interact() {
            if (!station) {
                Debug.LogError("[!] Cannot possess a null station!");
                return;
            }
            
            if (!station.occupied) {
                PlayerCharacter.ownedCharacter.PossessStation(station);
            }
        }
    }
}