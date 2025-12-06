using UnityEngine;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public class InterfaceSwitch : BoolControl {
        protected override void SetHandleTransform(float amount) {
            handle.localRotation = Quaternion.Euler(amount, 0, 0);
        }
    }
}