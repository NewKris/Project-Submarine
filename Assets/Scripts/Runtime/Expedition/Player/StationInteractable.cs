using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition.Player {
    public class StationInteractable : Interactable {
        public Station station;
        
        public override void Interact() {
            PlayerCharacter.ownedCharacter.PossessStation(station);
        }
    }
}