using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WereHorse.Runtime.Ui {
    public class MenuController : MonoBehaviour {
        public void JoinLobbyAsOffline() {
            SceneManager.LoadScene("Lobby");
        }
        
        public void JoinLobbyAsClient() {
            NetworkManager.Singleton.StartClient();
        }
        
        public void JoinLobbyAsHost() {
            Debug.Log("Starting Host");
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        
        public void ExitGame() {
            GameManager.ExitGame();
        }
    }
}