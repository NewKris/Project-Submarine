using UnityEngine;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceButton : BoolControl {
        protected override void SetHandleTransform(float amount) {
            handle.localPosition = new Vector3(0, amount, 0);
        }
    }
}