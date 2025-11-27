using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Werehorse.Runtime.Utility.Attributes;
using Object = UnityEngine.Object;

namespace Werehorse.Editor {
    [CustomPropertyDrawer(typeof(InspectorButton))]
    public class InspectorButtonDrawer : PropertyDrawer {
        private const float BUTTON_MARGIN = 15;
        private const float BUTTON_HEIGHT = 20;
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect buttonRect = position;
            buttonRect.y += BUTTON_MARGIN;
            buttonRect.height = BUTTON_HEIGHT;

            InspectorButton inspectorButton = (InspectorButton)attribute;
            
            if (GUI.Button(buttonRect, inspectorButton.displayName)) {
                RunMethod(inspectorButton.methodName, property.serializedObject.targetObject);
            }

            Rect propertyRect = position;
            propertyRect.y += ButtonRectHeight();
            propertyRect.height = base.GetPropertyHeight(property, label);
            
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label) + ButtonRectHeight();
        }

        private float ButtonRectHeight() {
            return BUTTON_HEIGHT + BUTTON_MARGIN + BUTTON_MARGIN;
        }

        private void RunMethod(string methodName, Object sourceObject) {
            try {
                sourceObject
                    .GetType()
                    .GetMethod(methodName, BINDING_FLAGS)
                    .Invoke(sourceObject, Array.Empty<object>());
            }
            catch (Exception e) {
                Debug.LogError("Failed to invoke " + methodName + ": " + e.Message);
            }
        }
    }
}
