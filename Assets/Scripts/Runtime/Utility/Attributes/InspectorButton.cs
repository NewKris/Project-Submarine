using UnityEngine;

namespace WereHorse.Runtime.Utility.Attributes {
    public class InspectorButton : PropertyAttribute {
        public string methodName;
        public string displayName;
        
        public InspectorButton(string methodName, string displayName) {
            this.methodName = methodName;
            this.displayName = displayName;
        }
    }
}
