using System;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Gameplay.Expedition {
    public class ExpeditionController : NetworkBehaviourExtended {
        private void Start() {
            DoOnServer(() => {
                
            });
        }
    }
}