using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Gameplay.Lobby {
    public class LobbyController : MonoBehaviour {
        private void OnGUI() {
            GUILayout.BeginArea(new Rect(300, 10, 100, 100));

            if (GUILayout.Button("Start Expedition")) {
                StartExpedition();
            }
            
            if (GUILayout.Button("Exit Lobby")) {
                ClientController.DisconnectFromGame();
            }
            
            GUILayout.EndArea();
        }

        public void StartExpedition() {
            if (NetworkManager.Singleton.IsServer) {
                NetworkManager.Singleton.SceneManager.LoadScene("Expedition", LoadSceneMode.Single);
            }
        }
    }
}