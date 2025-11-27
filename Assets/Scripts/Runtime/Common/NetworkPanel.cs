using System;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Common {
    public class NetworkPanel : MonoBehaviour {
        private static NetworkPanel Instance;

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            
            GUILayout.Label($"Playing as: {GameManager.clientType}");
            
            GUILayout.EndArea();
        }

        private void Awake() {
            Singleton.MakeSingleton(ref Instance, this);
        }

        private void OnDestroy() {
            Singleton.RemoveSingleton(ref Instance, this);
        }
    }
}