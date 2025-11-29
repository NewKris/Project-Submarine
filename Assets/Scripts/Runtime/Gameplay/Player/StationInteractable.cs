using WereHorse.Runtime.Gameplay.Interaction;
using WereHorse.Runtime.Gameplay.Player.Character;

namespace WereHorse.Runtime.Gameplay.Player {
    public class StationInteractable : Interactable {
        public Station station;
        
        public override void Interact() {
            PlayerCharacter.ownedCharacter.PossessStation(station);
        }
    }
}